import React from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import {
    Box, VStack, HStack, FormControl, FormLabel, Input, Select, Button,
    Card, CardHeader, CardBody, Heading, Divider, FormErrorMessage, SimpleGrid,
} from '@chakra-ui/react';
import {
    FaSave, FaUser, FaEnvelope, FaCalendarAlt, FaVenusMars, FaIdCard,
    FaGlobe, FaMapMarkerAlt, FaHome, FaRoad, FaBuilding, FaCity, FaMapPin,
} from 'react-icons/fa';
import * as yup from 'yup';
import { validarCPF, validarEmail } from '../../utils/validators';
import { maskCPF, maskCEP, unMask } from '../../utils/masks';
import { useNotification } from '../../hooks/useNotification';

const enderecoSchema = yup.object().shape({
    cep: yup.string().nullable(),
    logradouro: yup.string().nullable(),
    numero: yup.string().nullable(),
    complemento: yup.string().nullable(),
    bairro: yup.string().nullable(),
    cidade: yup.string().nullable(),
    estado: yup.string().nullable(),
});

const pessoaSchema = yup.object().shape({
    nome: yup.string().required('Nome é obrigatório'),
    sexo: yup.string().oneOf(['', '0', '1', '2'], 'Sexo inválido'),
    email: yup
        .string()
        .nullable()
        .transform((value) => (value === '' ? null : value))
        .test('is-email', 'E-mail inválido', (value) => {
            if (!value) return true;
            return validarEmail(value);
        }),
    dataNascimento: yup
        .string()
        .required('Data de nascimento é obrigatória')
        .test('is-date', 'Data de nascimento inválida', (value) => {
            if (!value) return false;
            const date = new Date(value);
            return !isNaN(date.getTime());
        }),
    naturalidade: yup.string().nullable(),
    nacionalidade: yup.string().nullable(),
    cpf: yup
        .string()
        .required('CPF é obrigatório')
        .test('is-cpf', 'CPF inválido', (value) => validarCPF(value)),
    endereco: enderecoSchema,
});

export default function PessoaForm({
    initialData = null,
    onSubmit,
    onCancel,
    isLoading = false,
    submitButtonText = "Salvar Pessoa",
    title = "Dados Pessoais"
}) {
    const { showError } = useNotification();

    const formatDateForInput = (dateISO) => {
        if (!dateISO) return '';

        try {
            if (typeof dateISO === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(dateISO)) {
                return dateISO;
            }

            if (typeof dateISO === 'string' && dateISO.includes('T')) {
                return dateISO.split('T')[0];
            }

            const date = new Date(dateISO);
            if (isNaN(date.getTime())) return '';

            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');

            return `${year}-${month}-${day}`;
        } catch (error) {
            return '';
        }
    };

    const getDefaultValues = () => ({
        nome: initialData?.Nome || '',
        email: initialData?.Email || '',
        dataNascimento: formatDateForInput(initialData?.DataNascimento),
        sexo: initialData?.Sexo?.toString() || '',
        cpf: initialData?.Cpf || '',
        naturalidade: initialData?.Naturalidade || '',
        nacionalidade: initialData?.Nacionalidade || '',
        endereco: {
            cep: initialData?.Endereco?.Cep || '',
            logradouro: initialData?.Endereco?.Logradouro || '',
            numero: initialData?.Endereco?.Numero || '',
            complemento: initialData?.Endereco?.Complemento || '',
            bairro: initialData?.Endereco?.Bairro || '',
            cidade: initialData?.Endereco?.Cidade || '',
            estado: initialData?.Endereco?.Estado || '',
        },
    });

    const cleanString = (str) => str && str.trim() !== '' ? str.trim() : '';

    const hasAddressData = (endereco) => {
        return endereco && (
            cleanString(endereco.cep) ||
            cleanString(endereco.logradouro) ||
            cleanString(endereco.numero) ||
            cleanString(endereco.complemento) ||
            cleanString(endereco.bairro) ||
            cleanString(endereco.cidade) ||
            cleanString(endereco.estado)
        );
    };

    const buildAddressData = (endereco) => ({
        CEP: endereco?.cep ? maskCEP(unMask(endereco.cep)) : '',
        Logradouro: cleanString(endereco?.logradouro) || '',
        Numero: cleanString(endereco?.numero) || '',
        Complemento: cleanString(endereco?.complemento) || '',
        Bairro: cleanString(endereco?.bairro) || '',
        Cidade: cleanString(endereco?.cidade) || '',
        Estado: cleanString(endereco?.estado)?.toUpperCase() || '',
    });

    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        watch,
        reset,
    } = useForm({
        resolver: yupResolver(pessoaSchema),
        mode: 'onChange',
        defaultValues: getDefaultValues(),
    });

    const cpfValue = watch('cpf');
    const cepValue = watch('endereco.cep');

    React.useEffect(() => {
        if (initialData) {
            const newValues = {
                ...getDefaultValues(),
                dataNascimento: formatDateForInput(initialData?.DataNascimento),
            };
            reset(newValues);
        }
    }, [initialData, reset]);

    React.useEffect(() => {
        if (cpfValue) {
            const formatted = maskCPF(cpfValue);
            if (formatted !== cpfValue) {
                setValue('cpf', formatted);
            }
        }
    }, [cpfValue, setValue]);

    React.useEffect(() => {
        if (cepValue) {
            const formatted = maskCEP(cepValue);
            if (formatted !== cepValue) {
                setValue('endereco.cep', formatted);
            }
        }
    }, [cepValue, setValue]);

    const handleFormSubmit = async (data) => {
        try {
            const pessoaData = {
                Nome: cleanString(data.nome),
                Email: cleanString(data.email),
                DataNascimento: data.dataNascimento,
                Sexo: data.sexo ? parseInt(data.sexo) : 0,
                CPF: data.cpf ? maskCPF(unMask(data.cpf)) : '',
                Naturalidade: cleanString(data.naturalidade),
                Nacionalidade: cleanString(data.nacionalidade),
            };

            if (initialData?.Id) {
                pessoaData.Id = initialData.Id;
            }

            const endereco = data.endereco;
            const temEndereco = hasAddressData(endereco);

            if (!initialData?.Id) {
                pessoaData.Endereco = {
                    Id: null,
                    ...buildAddressData(endereco),
                };
            } else if (temEndereco) {
                pessoaData.Endereco = buildAddressData(endereco);

                if (initialData?.Endereco?.Id) {
                    pessoaData.Endereco.Id = initialData.Endereco.Id;
                }
            }

            await onSubmit(pessoaData);
        } catch (error) {
            let errorMessage = 'Não foi possível processar a solicitação. Tente novamente.';

            if (error.response?.data?.message) {
                errorMessage = error.response.data.message;
            } else if (error.response?.data?.errors) {
                errorMessage = Object.values(error.response.data.errors).flat().join(', ');
            }

            showError(errorMessage);
        }
    };

    const FormField = ({ icon: Icon, label, children, error, required = false }) => (
        <FormControl isInvalid={!!error}>
            <FormLabel display="flex" alignItems="center">
                <Box as={Icon} mr={2} color="gray.500" />
                {label} {required && '*'}
            </FormLabel>
            {children}
            <FormErrorMessage>{error?.message}</FormErrorMessage>
        </FormControl>
    );

    return (
        <Card shadow="md">
            <CardHeader>
                <Heading size="md" display="flex" alignItems="center">
                    <Box as={FaUser} mr={2} />
                    {title}
                </Heading>
            </CardHeader>
            <CardBody>
                <form onSubmit={handleSubmit(handleFormSubmit)}>
                    <VStack spacing={6} align="stretch">
                        <FormField icon={FaUser} label="Nome Completo" error={errors.nome} required>
                            <Input
                                {...register('nome')}
                                placeholder="Digite o nome completo"
                                size="lg"
                            />
                        </FormField>

                        <FormField icon={FaIdCard} label="CPF" error={errors.cpf} required>
                            <Input
                                {...register('cpf')}
                                placeholder="000.000.000-00"
                                size="lg"
                                maxLength={14}
                            />
                        </FormField>

                        <FormField icon={FaEnvelope} label="Email" error={errors.email}>
                            <Input
                                {...register('email')}
                                type="email"
                                placeholder="Digite o email"
                                size="lg"
                            />
                        </FormField>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                            <FormField icon={FaCalendarAlt} label="Data de Nascimento" error={errors.dataNascimento} required>
                                <Input
                                    {...register('dataNascimento')}
                                    type="date"
                                    size="lg"
                                />
                            </FormField>

                            <FormField icon={FaVenusMars} label="Sexo" error={errors.sexo}>
                                <Select
                                    {...register('sexo')}
                                    placeholder="Selecione o sexo"
                                    size="lg"
                                >
                                    <option value="1">Masculino</option>
                                    <option value="2">Feminino</option>
                                    <option value="0">Não informado</option>
                                </Select>
                            </FormField>
                        </SimpleGrid>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                            <FormField icon={FaMapMarkerAlt} label="Naturalidade" error={errors.naturalidade}>
                                <Input
                                    {...register('naturalidade')}
                                    placeholder="Ex: São Paulo"
                                    size="lg"
                                />
                            </FormField>

                            <FormField icon={FaGlobe} label="Nacionalidade" error={errors.nacionalidade}>
                                <Input
                                    {...register('nacionalidade')}
                                    placeholder="Ex: Brasileira"
                                    size="lg"
                                />
                            </FormField>
                        </SimpleGrid>

                        <Divider />
                        <Box>
                            <Heading size="sm" display="flex" alignItems="center" mb={4}>
                                <Box as={FaHome} mr={2} color="gray.500" />
                                Endereço (Opcional)
                            </Heading>

                            <VStack spacing={4} align="stretch">
                                <FormField icon={FaMapPin} label="CEP" error={errors.endereco?.cep}>
                                    <Input
                                        {...register('endereco.cep')}
                                        placeholder="00000-000"
                                        size="lg"
                                        maxLength={9}
                                    />
                                </FormField>

                                <FormField icon={FaRoad} label="Logradouro" error={errors.endereco?.logradouro}>
                                    <Input
                                        {...register('endereco.logradouro')}
                                        placeholder="Ex: Rua das Flores"
                                        size="lg"
                                    />
                                </FormField>

                                <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
                                    <FormField icon={FaBuilding} label="Número" error={errors.endereco?.numero}>
                                        <Input
                                            {...register('endereco.numero')}
                                            placeholder="123"
                                            size="lg"
                                        />
                                    </FormField>

                                    <FormControl isInvalid={!!errors.endereco?.complemento}>
                                        <FormLabel>Complemento</FormLabel>
                                        <Input
                                            {...register('endereco.complemento')}
                                            placeholder="Apto 101"
                                            size="lg"
                                        />
                                        <FormErrorMessage>
                                            {errors.endereco?.complemento?.message}
                                        </FormErrorMessage>
                                    </FormControl>

                                    <FormField icon={FaMapMarkerAlt} label="Bairro" error={errors.endereco?.bairro}>
                                        <Input
                                            {...register('endereco.bairro')}
                                            placeholder="Centro"
                                            size="lg"
                                        />
                                    </FormField>
                                </SimpleGrid>

                                <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                                    <FormField icon={FaCity} label="Cidade" error={errors.endereco?.cidade}>
                                        <Input
                                            {...register('endereco.cidade')}
                                            placeholder="São Paulo"
                                            size="lg"
                                        />
                                    </FormField>

                                    <FormControl isInvalid={!!errors.endereco?.estado}>
                                        <FormLabel>Estado (UF)</FormLabel>
                                        <Input
                                            {...register('endereco.estado')}
                                            placeholder="SP"
                                            size="lg"
                                            maxLength={2}
                                            style={{ textTransform: 'uppercase' }}
                                        />
                                        <FormErrorMessage>
                                            {errors.endereco?.estado?.message}
                                        </FormErrorMessage>
                                    </FormControl>
                                </SimpleGrid>
                            </VStack>
                        </Box>

                        <Divider />
                        <HStack justify="flex-end" spacing={4}>
                            <Button
                                variant="outline"
                                onClick={onCancel}
                                disabled={isLoading}
                            >
                                Cancelar
                            </Button>
                            <Button
                                type="submit"
                                colorScheme="blue"
                                leftIcon={<FaSave />}
                                isLoading={isLoading}
                                loadingText="Salvando..."
                            >
                                {submitButtonText}
                            </Button>
                        </HStack>
                    </VStack>
                </form>
            </CardBody>
        </Card>
    );
}