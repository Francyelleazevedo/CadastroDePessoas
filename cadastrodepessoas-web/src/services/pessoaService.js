import api, { resolveApiPath } from './api';

export const pessoaService = {
    listar: async () => {
        try {
            const response = await api.get(resolveApiPath('Pessoa'));
            return response.data;
        } catch (error) {
            console.error('Erro ao listar pessoas:', error);
            throw error;
        }
    },

    obterPorId: async (id) => {
        try {
            const response = await api.get(resolveApiPath(`Pessoa/${id}`));
            return response.data;
        } catch (error) {
            console.error(`Erro ao obter pessoa com ID ${id}:`, error);
            throw error;
        }
    },

    criar: async (pessoa) => {
        try {
            const response = await api.post(resolveApiPath('Pessoa'), pessoa);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar pessoa:', error);
            throw error;
        }
    },

    atualizar: async (pessoa) => {
        try {
            const payload = {
                ...pessoa,
                id: pessoa.id
            };
            const response = await api.put(resolveApiPath('Pessoa'), payload);
            return response.data;
        } catch (error) {
            console.error(`Erro ao atualizar pessoa com ID ${pessoa.id}:`, error);
            throw error;
        }
    },

    deletar: async (id) => {
        try {
            const response = await api.delete(resolveApiPath(`Pessoa/${id}`));
            return response.data;
        } catch (error) {
            console.error(`Erro ao deletar pessoa com ID ${id}:`, error);
            throw error;
        }
    },
};