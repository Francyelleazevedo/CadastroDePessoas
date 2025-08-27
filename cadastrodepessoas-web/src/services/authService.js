
import api, { resolveApiPath } from './api';

export const authService = {
    login: async (email, senha) => {
        try {
            const response = await api.post(resolveApiPath('Auth/login'), {
                email,
                senha
            });

            if (response.data.success && response.data.token) {
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('user', JSON.stringify(response.data.user));

                return {
                    success: true,
                    token: response.data.token,
                    user: response.data.user,
                    message: response.data.message
                };
            }

            return {
                success: false,
                message: response.data.message || 'Erro no login'
            };
        } catch (error) {

            if (error.response?.data?.message) {
                return {
                    success: false,
                    message: error.response.data.message
                };
            }

            if (error.response?.status === 401) {
                return {
                    success: false,
                    message: 'Email ou senha incorretos'
                };
            }

            return {
                success: false,
                message: 'Erro de conexão. Verifique se o servidor está rodando.'
            };
        }
    },

    registrar: async (nome, email, senha) => {
        try {
            const response = await api.post(resolveApiPath('Auth/registrar'), {
                nome,
                email,
                senha
            });
            if (response.data.success && response.data.token) {
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('user', JSON.stringify(response.data.user));
                return response.data;
            }
            return {
                success: false,
                message: response.data.message || 'Erro no registro'
            };
        } catch (error) {
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            if (error.response?.status === 401) {
                throw new Error('Erro ao registrar. Verifique as credenciais.');
            }
            throw new Error('Erro de conexão. Verifique se o servidor está rodando.');
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        return Promise.resolve();
    },
}
