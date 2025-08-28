import React from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import {
    Box,
    VStack,
    HStack,
    FormControl,
    FormLabel,
    Input,
    Select,
    Button,
    Card,
    CardHeader,
    CardBody,
    Heading,
    Divider,
    FormErrorMessage,
    SimpleGrid,
} from '@chakra-ui/react';
import {
    FaSave,
    FaUser,
    FaEnvelope,
    FaCalendarAlt,
    FaVenusMars,
    FaIdCard,
    FaGlobe,
    FaMapMarkerAlt,
    FaHome,
    FaRoad,
    FaBuilding,
    FaCity,
    FaMapPin,
} from 'react-icons/fa';
import * as yup from 'yup';
import { validarCPF, validarEmail } from '../../utils/validators';
import { maskCPF, maskCEP, unMask } from '../../utils/masks';
import { useNotification } from '../../hooks/useNotification';

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
    endereco: yup.object().shape({
        cep: yup.string().nullable(),
        logradouro: yup.string().nullable(),
        numero: yup.string().nullable(),
        complemento: yup.string().nullable(),
        bairro: yup.string().nullable(),
        cidade: yup.string().nullable(),
        estado: yup.string().nullable(),
    }),
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

    const formatarDataParaInput = (dataISO) => {
        if (!dataISO) return '';
        
        try {
            if (typeof dataISO === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(dataISO)) {
                return dataISO;
            }
            
            if (typeof dataISO === 'string' && dataISO.includes('T')) {
                const dataFormatada = dataISO.split('T')[0];
                return dataFormatada;
            }
            
            const data = new Date(dataISO);
            
            if (isNaN(data.getTime())) {
                return '';
            }
            
            const ano = data.getFullYear();
            const mes = String(data.getMonth() + 1).padStart(2, '0');
            const dia = String(data.getDate()).padStart(2, '0');
            
            const dataFormatada = `${ano}-${mes}-${dia}`;
            
            return dataFormatada;
        } catch (error) {
            return '';
        }
    };

    const defaultValues = {
        nome: initialData?.Nome || '',
        email: initialData?.Email || '',
        dataNascimento: formatarDataParaInput(initialData?.DataNascimento),
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
    };

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
        defaultValues,
    });

    React.useEffect(() => {
        if (initialData) {
            const dataFormatada = formatarDataParaInput(initialData?.DataNascimento);
            
            const newValues = {
                nome: initialData?.Nome || '',
                email: initialData?.Email || '',
                dataNascimento: dataFormatada,
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
            };
            reset(newValues);
        }
    }, [initialData, reset]);

    const cpfValue = watch('cpf');
    const cepValue = watch('endereco.cep');

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
            const cleanString = (str) => str && str.trim() !== '' ? str.trim() : '';
            
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
            const temEndereco = endereco && (
                cleanString(endereco.cep) ||
                cleanString(endereco.logradouro) ||
                cleanString(endereco.numero) ||
                cleanString(endereco.complemento) ||
                cleanString(endereco.bairro) ||
                cleanString(endereco.cidade) ||
                cleanString(endereco.estado)
            );

            if (!initialData?.Id) {
                pessoaData.Endereco = {
                    Id: null,
                    CEP: endereco?.cep ? maskCEP(unMask(endereco.cep)) : '',
                    Logradouro: cleanString(endereco?.logradouro) || '',
                    Numero: cleanString(endereco?.numero) || '',
                    Complemento: cleanString(endereco?.complemento) || '',
                    Bairro: cleanString(endereco?.bairro) || '',
                    Cidade: cleanString(endereco?.cidade) || '',
                    Estado: cleanString(endereco?.estado)?.toUpperCase() || '',
                };
            } else if (temEndereco) {
                pessoaData.Endereco = {
                    CEP: endereco.cep ? maskCEP(unMask(endereco.cep)) : '',
                    Logradouro: cleanString(endereco.logradouro),
                    Numero: cleanString(endereco.numero),
                    Complemento: cleanString(endereco.complemento),
                    Bairro: cleanString(endereco.bairro),
                    Cidade: cleanString(endereco.cidade),
                    Estado: cleanString(endereco.estado)?.toUpperCase() || '',
                };

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
                        <FormControl isInvalid={!!errors.nome}>
                            <FormLabel display="flex" alignItems="center">
                                <Box as={FaUser} mr={2} color="gray.500" />
                                Nome Completo *
                            </FormLabel>
                            <Input
                                {...register('nome')}
                                placeholder="Digite o nome completo"
                                size="lg"
                            />
                            <FormErrorMessage>
                                {errors.nome?.message}
                            </FormErrorMessage>
                        </FormControl>

                        <FormControl isInvalid={!!errors.cpf}>
                            <FormLabel display="flex" alignItems="center">
                                <Box as={FaIdCard} mr={2} color="gray.500" />
                                CPF *
                            </FormLabel>
                            <Input
                                {...register('cpf')}
                                placeholder="000.000.000-00"
                                size="lg"
                                maxLength={14}
                            />
                            <FormErrorMessage>
                                {errors.cpf?.message}
                            </FormErrorMessage>
                        </FormControl>

                        <FormControl isInvalid={!!errors.email}>
                            <FormLabel display="flex" alignItems="center">
                                <Box as={FaEnvelope} mr={2} color="gray.500" />
                                Email
                            </FormLabel>
                            <Input
                                {...register('email')}
                                type="email"
                                placeholder="Digite o email"
                                size="lg"
                            />
                            <FormErrorMessage>
                                {errors.email?.message}
                            </FormErrorMessage>
                        </FormControl>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                            <FormControl isInvalid={!!errors.dataNascimento}>
                                <FormLabel display="flex" alignItems="center">
                                    <Box as={FaCalendarAlt} mr={2} color="gray.500" />
                                    Data de Nascimento *
                                </FormLabel>
                                <Input
                                    {...register('dataNascimento')}
                                    type="date"
                                    size="lg"
                                />
                                <FormErrorMessage>
                                    {errors.dataNascimento?.message}
                                </FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={!!errors.sexo}>
                                <FormLabel display="flex" alignItems="center">
                                    <Box as={FaVenusMars} mr={2} color="gray.500" />
                                    Sexo
                                </FormLabel>
                                <Select
                                    {...register('sexo')}
                                    placeholder="Selecione o sexo"
                                    size="lg"
                                >
                                    <option value="1">Masculino</option>
                                    <option value="2">Feminino</option>
                                    <option value="0">Não informado</option>
                                </Select>
                                <FormErrorMessage>
                                    {errors.sexo?.message}
                                </FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                            <FormControl isInvalid={!!errors.naturalidade}>
                                <FormLabel display="flex" alignItems="center">
                                    <Box as={FaMapMarkerAlt} mr={2} color="gray.500" />
                                    Naturalidade
                                </FormLabel>
                                <Input
                                    {...register('naturalidade')}
                                    placeholder="Ex: São Paulo"
                                    size="lg"
                                />
                                <FormErrorMessage>
                                    {errors.naturalidade?.message}
                                </FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={!!errors.nacionalidade}>
                                <FormLabel display="flex" alignItems="center">
                                    <Box as={FaGlobe} mr={2} color="gray.500" />
                                    Nacionalidade
                                </FormLabel>
                                <Input
                                    {...register('nacionalidade')}
                                    placeholder="Ex: Brasileira"
                                    size="lg"
                                />
                                <FormErrorMessage>
                                    {errors.nacionalidade?.message}
                                </FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>

                        <Divider />

                        <Box>
                            <Heading size="sm" display="flex" alignItems="center" mb={4}>
                                <Box as={FaHome} mr={2} color="gray.500" />
                                Endereço (Opcional)
                            </Heading>

                            <VStack spacing={4} align="stretch">
                                <FormControl isInvalid={!!errors.endereco?.cep}>
                                    <FormLabel display="flex" alignItems="center">
                                        <Box as={FaMapPin} mr={2} color="gray.500" />
                                        CEP
                                    </FormLabel>
                                    <Input
                                        {...register('endereco.cep')}
                                        placeholder="00000-000"
                                        size="lg"
                                        maxLength={9}
                                    />
                                    <FormErrorMessage>
                                        {errors.endereco?.cep?.message}
                                    </FormErrorMessage>
                                </FormControl>

                                <FormControl isInvalid={!!errors.endereco?.logradouro}>
                                    <FormLabel display="flex" alignItems="center">
                                        <Box as={FaRoad} mr={2} color="gray.500" />
                                        Logradouro
                                    </FormLabel>
                                    <Input
                                        {...register('endereco.logradouro')}
                                        placeholder="Ex: Rua das Flores"
                                        size="lg"
                                    />
                                    <FormErrorMessage>
                                        {errors.endereco?.logradouro?.message}
                                    </FormErrorMessage>
                                </FormControl>

                                <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
                                    <FormControl isInvalid={!!errors.endereco?.numero}>
                                        <FormLabel display="flex" alignItems="center">
                                            <Box as={FaBuilding} mr={2} color="gray.500" />
                                            Número
                                        </FormLabel>
                                        <Input
                                            {...register('endereco.numero')}
                                            placeholder="123"
                                            size="lg"
                                        />
                                        <FormErrorMessage>
                                            {errors.endereco?.numero?.message}
                                        </FormErrorMessage>
                                    </FormControl>

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

                                    <FormControl isInvalid={!!errors.endereco?.bairro}>
                                        <FormLabel display="flex" alignItems="center">
                                            <Box as={FaMapMarkerAlt} mr={2} color="gray.500" />
                                            Bairro
                                        </FormLabel>
                                        <Input
                                            {...register('endereco.bairro')}
                                            placeholder="Centro"
                                            size="lg"
                                        />
                                        <FormErrorMessage>
                                            {errors.endereco?.bairro?.message}
                                        </FormErrorMessage>
                                    </FormControl>
                                </SimpleGrid>

                                <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4}>
                                    <FormControl isInvalid={!!errors.endereco?.cidade}>
                                        <FormLabel display="flex" alignItems="center">
                                            <Box as={FaCity} mr={2} color="gray.500" />
                                            Cidade
                                        </FormLabel>
                                        <Input
                                            {...register('endereco.cidade')}
                                            placeholder="São Paulo"
                                            size="lg"
                                        />
                                        <FormErrorMessage>
                                            {errors.endereco?.cidade?.message}
                                        </FormErrorMessage>
                                    </FormControl>

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