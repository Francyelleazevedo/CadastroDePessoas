// Função para calcular idade a partir da data de nascimento
export const calcularIdade = (dataNascimento) => {
    if (!dataNascimento) return null;
    
    try {
        const hoje = new Date();
        const nascimento = new Date(dataNascimento);
        
        // Verifica se a data é válida
        if (isNaN(nascimento.getTime())) return null;
        
        let idade = hoje.getFullYear() - nascimento.getFullYear();
        const mesAtual = hoje.getMonth();
        const mesNascimento = nascimento.getMonth();
        
        // Ajusta a idade se ainda não fez aniversário este ano
        if (mesAtual < mesNascimento || (mesAtual === mesNascimento && hoje.getDate() < nascimento.getDate())) {
            idade--;
        }
        
        return idade >= 0 ? idade : null;
    } catch (error) {
        return null;
    }
};

/**
 * Formata uma data ISO para o formato do input date (YYYY-MM-DD)
 * @param {string} dataISO 
 * @returns {string}
 */
export const formatarDataParaInput = (dataISO) => {
    if (!dataISO) return '';
    
    try {
        // Se já está no formato correto
        if (typeof dataISO === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(dataISO)) {
            return dataISO;
        }
        
        const data = new Date(dataISO);
        
        if (isNaN(data.getTime())) return '';
        
        const ano = data.getUTCFullYear();
        const mes = String(data.getUTCMonth() + 1).padStart(2, '0');
        const dia = String(data.getUTCDate()).padStart(2, '0');
        
        return `${ano}-${mes}-${dia}`;
    } catch (error) {
        console.error('Erro ao formatar data:', error);
        return '';
    }
};
