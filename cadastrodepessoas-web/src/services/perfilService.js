import api, { resolveApiPath } from './api';

export const perfilService = {
    obterPerfil: async () => {
        try {
            const url = resolveApiPath('Auth/perfil');
            const response = await api.get(url);
            return response.data;
        } catch (error) {
            console.error('Erro ao obter perfil:', error);
            throw error;
        }
    },

    alterarSenha: async (dadosAlteracaoSenha) => {
        try {
            const url = resolveApiPath('Auth/alterar-senha');
            const response = await api.put(url, dadosAlteracaoSenha);
            return response.data;
        } catch (error) {
            console.error('Erro ao alterar senha:', error);
            throw error;
        }
    },
};
