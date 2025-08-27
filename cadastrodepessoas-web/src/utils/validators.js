export const validarCPF = (cpf) => {
    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
        return false;
    }

    if (/^(\d)\1+$/.test(cpf)) {
        return false;
    }

    let soma = 0;
    for (let i = 0; i < 9; i++) {
        soma += parseInt(cpf.charAt(i)) * (10 - i);
    }

    let resto = 11 - (soma % 11);
    let dv1 = resto > 9 ? 0 : resto;

    soma = 0;
    for (let i = 0; i < 10; i++) {
        soma += parseInt(cpf.charAt(i)) * (11 - i);
    }

    resto = 11 - (soma % 11);
    let dv2 = resto > 9 ? 0 : resto;

    return (parseInt(cpf.charAt(9)) === dv1 && parseInt(cpf.charAt(10)) === dv2);
};

export const validarCEP = (cep) => {
    if (!cep || typeof cep !== 'string') return false;
    const digits = cep.replace(/\D/g, '');
    return digits.length === 8;
};

export const validarEmail = (email) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
};

export const validarDataNascimento = (data) => {
    const dataNascimento = new Date(data);
    const hoje = new Date();

    return dataNascimento < hoje && dataNascimento > new Date('1900-01-01');
};