export const formatarData = (data) => {
    if (!data) return 'N/A';
    try {
        return new Date(data).toLocaleDateString('pt-BR');
    } catch (error) {
        return 'N/A';
    }
};

export const formatarCPF = (cpf) => {
    if (!cpf) return '';

    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
        return cpf;
    }

    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
};

export const formatarEndereco = (endereco) => {
    if (!endereco) return '';
    
    const logradouro = endereco.Logradouro || endereco.logradouro || '';
    const numero = endereco.Numero || endereco.numero || '';
    const complemento = endereco.Complemento || endereco.complemento || '';
    const bairro = endereco.Bairro || endereco.bairro || '';
    const cidade = endereco.Cidade || endereco.cidade || '';
    const estado = endereco.Estado || endereco.estado || '';
    const cep = endereco.CEP || endereco.cep || '';
    
    if (logradouro && numero) {
        return `${logradouro}, ${numero}${complemento ? ', ' + complemento : ''}, ${bairro}, ${cidade} - ${estado}, ${cep}`;
    }
    
    return '';
};

export const formatarCEP = (cep) => {
    if (!cep) return '';
    const digits = cep.replace(/\D/g, ''); 
    if (digits.length !== 8) return cep;
    return `${digits.substring(0, 5)}-${digits.substring(5, 8)}`;
};