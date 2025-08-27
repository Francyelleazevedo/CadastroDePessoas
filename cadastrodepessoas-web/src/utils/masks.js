/**
 * Aplica máscara de CPF: 000.000.000-00
 * @param {string} value
 * @returns {string}
 */
export const maskCPF = (value) => {
    if (!value) return '';
    const digits = value.replace(/\D/g, '').substring(0, 11);
    return digits
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
};

/**
 * Aplica máscara de CEP: 00000-000
 * @param {string} value
 * @returns {string}
 */
export const maskCEP = (value) => {
    if (!value) return '';
    const digits = value.replace(/\D/g, '').substring(0, 8);
    return digits.replace(/(\d{5})(\d{1,3})/, '$1-$2');
};

/**
 * Aplica máscara de data de nascimento: 00/00/0000
 * @param {string} value
 * @returns {string}
 */
export const maskDate = (value) => {
    if (!value) return '';
    const digits = value.replace(/\D/g, '').substring(0, 8);
    return digits
        .replace(/(\d{2})(\d)/, '$1/$2')
        .replace(/(\d{2})(\d)/, '$1/$2')
        .replace(/(\d{4})$/, '$1');
};

/**
 * Remove qualquer máscara (deixa só números)
 * @param {string} value
 * @returns {string}
 */
export const unMask = (value) => {
    if (!value) return '';
    return value.replace(/\D/g, '');
};
