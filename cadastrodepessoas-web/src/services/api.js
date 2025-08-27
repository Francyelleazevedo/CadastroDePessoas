import axios from 'axios';

let notificationService = null;
let navigateFunction = null;

export const setupApiErrorHandling = (notificationHook, navigate) => {
    notificationService = notificationHook;
    navigateFunction = navigate;
};

const getBaseURL = () => {
    const isDevelopment = import.meta.env.DEV;
    
    if (isDevelopment) {
        return import.meta.env.VITE_API_URL || 'https://localhost:5001';
    }
    
    const prodBaseUrl = import.meta.env.VITE_API_URL || 'https://cadastro-de-pessoas-15rn.onrender.com';
    return prodBaseUrl.endsWith('/api/v1') ? prodBaseUrl : prodBaseUrl;
};

const resolveApiPath = (path) => {
    if (path.startsWith('/api/v1')) {
        return path;
    }    
    const cleanPath = path.startsWith('/') ? path.substring(1) : path;    
    return `/api/v1/${cleanPath}`;
};

const api = axios.create({
    baseURL: getBaseURL(),
    headers: {
        'Content-Type': 'application/json; charset=utf-8',
    },
    timeout: 30000,
});

const publicEndpoints = [
    '/api/v1/Auth/login',
    '/api/v1/Auth/registrar',
    '/api/v1/Auth/logout',
    '/api/v1/Auth/reset-admin',
    '/Health',
    '/Health/database',
    '/Health/reset-database'
];

const isTokenExpired = (token) => {
    if (!token) return true;
    
    try {
        const parts = token.split('.');
        if (parts.length !== 3) return true;
        
        const payload = parts[1];
        const decodedPayload = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
        
        if (!decodedPayload.exp) return true;
        
        const currentTime = Math.floor(Date.now() / 1000) - 30;
        return decodedPayload.exp < currentTime;
    } catch (error) {
        console.error('Erro ao verificar expiração do token:', error);
        return true;
    }
};

const getValidToken = () => {
    const token = localStorage.getItem('token');
    
    if (!token || isTokenExpired(token)) {
        if (window.location.pathname !== '/login' && 
            window.location.pathname !== '/registrar' && 
            window.location.pathname !== '/' &&
            !window.location.pathname.includes('/pessoas')) {
            
            if (notificationService) {
                notificationService.showWarning(
                    'Sua sessão expirou ou não foi encontrada. Por favor, faça login novamente.',
                    'Sessão Expirada'
                );
            }
        }
        return null;
    }
    
    return token;
};

const showNotification = (type, message, title) => {
    if (notificationService) {
        switch (type) {
            case 'error':
                notificationService.showError(message, title);
                break;
            case 'warning':
                notificationService.showWarning(message, title);
                break;
            case 'info':
                notificationService.showInfo(message, title);
                break;
            case 'success':
                notificationService.showSuccess(message, title);
                break;
            default:
                notificationService.showInfo(message, title);
        }
    }
};

const redirectTo = (path, options = {}) => {
    if (navigateFunction) {
        navigateFunction(path, options);
    } else {
        window.location.href = path;
    }
};

api.interceptors.request.use(
    (config) => {
        const isPublicEndpoint = publicEndpoints.some(endpoint => 
            config.url?.includes(endpoint)
        );
        
        if (!isPublicEndpoint) {
            const token = getValidToken();
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            } else {
                showNotification('warning', 'Autenticação necessária para acessar este recurso.', 'Acesso Restrito');
            }
        }
        
        return config;
    },
    (error) => {
        showNotification('error', 'Erro ao preparar requisição para o servidor', 'Erro de Requisição');
        return Promise.reject(error);
    }
);

let refreshAttempts = 0;
const MAX_REFRESH_ATTEMPTS = 3;

api.interceptors.response.use(
    (response) => {
        refreshAttempts = 0;
        return response;
    },
    async (error) => {
        if (import.meta.env.DEV) {
            console.error(`API Error: ${error.response?.status}`, {
                url: error.config?.url,
                method: error.config?.method,
                data: error.response?.data,
                message: error.message
            });
        }

        if (!error.response) {
            showNotification('error', 'Não foi possível conectar ao servidor. Verifique sua conexão.', 'Erro de Conexão');
            redirectTo('/erro-conexao', { state: { offline: true } });
            return Promise.reject(error);
        }

        const { status, data } = error.response;
        const errorMessage = data?.message || 'Ocorreu um erro na comunicação com o servidor';
        const currentPath = window.location.pathname;
        
        if (status === 401) {
            const isLoginRequest = error.config?.url?.includes(resolveApiPath('Auth/login'));
            
            if (!isLoginRequest) {
                if (refreshAttempts < MAX_REFRESH_ATTEMPTS) {
                    refreshAttempts++;
                    
                    showNotification('warning', 'Sua sessão expirou. Por favor, faça login novamente.', 'Sessão Expirada');
                    
                    if (currentPath !== '/perfil' && 
                        !publicEndpoints.some(e => currentPath.includes(e.replace('/api/v1/', '')))) {
                        
                        localStorage.removeItem('token');
                        localStorage.removeItem('user');
                        
                        if (currentPath !== '/login' && currentPath !== '/sessao-expirada') {
                            redirectTo('/sessao-expirada', { state: { from: currentPath } });
                        }
                    }
                } else {
                    showNotification('error', 'Múltiplas falhas de autenticação. Por favor, faça login novamente.', 'Erro de Autenticação');
                    refreshAttempts = 0;
                    redirectTo('/login?error=auth');
                }
            }
        } 
        // Acesso negado
        else if (status === 403) {
            showNotification('error', data?.message || 'Você não tem permissão para acessar este recurso', 'Acesso Negado');
            if (currentPath !== '/acesso-negado') {
                redirectTo('/acesso-negado', { state: { from: currentPath } });
            }
        } 
        else if (status === 404) {
            showNotification('warning', 'O recurso solicitado não foi encontrado', 'Não Encontrado');
            const isErrorPage = ['/pagina-nao-encontrada', '/erro', '/acesso-negado', '/erro-conexao'].includes(currentPath);
            
            if (!error.config?.url?.includes('profile') && !isErrorPage) {
                const resource = error.config?.url?.includes('Pessoas') ? 'pessoa' : 'recurso';
                redirectTo('/pagina-nao-encontrada', { state: { resource } });
            }
        } 
        else if (status === 422) {
            showNotification('warning', 'Os dados fornecidos são inválidos', 'Dados Inválidos');
            
            if (data?.errors) {
                const errorDetails = Object.entries(data.errors)
                    .map(([field, errors]) => `${field}: ${errors.join(', ')}`)
                    .join('\n');
                
                showNotification('warning', errorDetails, 'Erros de Validação');
            }
        } 
        else if (status === 500) {
            showNotification('error', 'Ocorreu um erro no servidor. Tente novamente mais tarde.', 'Erro do Servidor');
            if (!['/erro', '/pagina-nao-encontrada', '/acesso-negado', '/erro-conexao'].includes(currentPath)) {
                redirectTo('/erro', { state: { errorMessage, statusCode: 500 } });
            }
        } 
        else {
            showNotification('error', `Erro HTTP ${status}: ${errorMessage}`, 'Erro de Comunicação');
        }

        return Promise.reject(error);
    }
);

export default api;
export { resolveApiPath };