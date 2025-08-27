import React, { useState } from 'react';
import {
    Box,
    VStack,
    FormControl,
    FormLabel,
    InputGroup,
    InputLeftElement,
    InputRightElement,
    Input,
    IconButton,
    Button,
    FormErrorMessage,
    Card,
    CardHeader,
    CardBody,
    Heading,
    Text,
} from '@chakra-ui/react';
import {
    FaLock,
    FaEye,
    FaEyeSlash,
    FaKey,
    FaSave,
} from 'react-icons/fa';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { perfilService } from '../../services/perfilService';
import { useNotification } from '../../hooks/useNotification';

const schema = yup.object().shape({
    senhaAtual: yup.string().required('Senha atual é obrigatória'),
    novaSenha: yup.string()
        .required('Nova senha é obrigatória')
        .min(8, 'A nova senha deve ter pelo menos 8 caracteres'),
    confirmarSenha: yup.string()
        .required('Confirmação de senha é obrigatória')
        .oneOf([yup.ref('novaSenha')], 'As senhas não coincidem'),
});

export default function AlterarSenhaForm({
    showTitle = true,
    onSuccess,
    cardStyle = true
}) {
    const { showSuccess, showError } = useNotification();
    const [showPasswords, setShowPasswords] = useState({
        atual: false,
        nova: false,
        confirmar: false,
    });

    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
        reset,
    } = useForm({
        resolver: yupResolver(schema),
        mode: 'onBlur',
    });

    const togglePasswordVisibility = (field) => {
        setShowPasswords(prev => ({
            ...prev,
            [field]: !prev[field]
        }));
    };

    const onSubmit = async (data) => {
        try {
            await perfilService.alterarSenha({
                senhaAtual: data.senhaAtual,
                novaSenha: data.novaSenha,
                confirmarSenha: data.confirmarSenha,
            });

            reset();

            if (onSuccess) {
                onSuccess();
            }
        } catch (error) {
            console.error('Erro ao alterar senha:', error); 
            let errorMessage = 'Não foi possível alterar a senha. Tente novamente.';
            if (error.response?.data?.message) {
                errorMessage = error.response.data.message;
            }

            showError(errorMessage);
        }
    };

    const FormContent = () => (
        <form onSubmit={handleSubmit(onSubmit)}>
            <VStack spacing={6}>
                {showTitle && (
                    <Box textAlign="center" mb={2}>
                        <Box
                            display="inline-flex"
                            alignItems="center"
                            justifyContent="center"
                            w={12}
                            h={12}
                            bg="blue.100"
                            borderRadius="full"
                            mb={3}
                        >
                            <FaKey color="#3182CE" size={20} />
                        </Box>
                        <Heading size="md" color="gray.700">
                            Alterar Senha
                        </Heading>
                        <Text color="gray.500" mt={1}>
                            Digite sua senha atual e a nova senha
                        </Text>
                    </Box>
                )}

                <FormControl isInvalid={!!errors.senhaAtual}>
                    <FormLabel>Senha Atual</FormLabel>
                    <InputGroup>
                        <InputLeftElement>
                            <FaLock color="gray.400" />
                        </InputLeftElement>
                        <Input
                            type={showPasswords.atual ? 'text' : 'password'}
                            placeholder="Digite sua senha atual"
                            {...register('senhaAtual')}
                        />
                        <InputRightElement>
                            <IconButton
                                variant="ghost"
                                size="sm"
                                aria-label={showPasswords.atual ? 'Ocultar senha' : 'Mostrar senha'}
                                icon={showPasswords.atual ? <FaEyeSlash /> : <FaEye />}
                                onClick={() => togglePasswordVisibility('atual')}
                            />
                        </InputRightElement>
                    </InputGroup>
                    <FormErrorMessage>{errors.senhaAtual?.message}</FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors.novaSenha}>
                    <FormLabel>Nova Senha</FormLabel>
                    <InputGroup>
                        <InputLeftElement>
                            <FaLock color="gray.400" />
                        </InputLeftElement>
                        <Input
                            type={showPasswords.nova ? 'text' : 'password'}
                            placeholder="Digite a nova senha (mín. 8 caracteres)"
                            {...register('novaSenha')}
                        />
                        <InputRightElement>
                            <IconButton
                                variant="ghost"
                                size="sm"
                                aria-label={showPasswords.nova ? 'Ocultar senha' : 'Mostrar senha'}
                                icon={showPasswords.nova ? <FaEyeSlash /> : <FaEye />}
                                onClick={() => togglePasswordVisibility('nova')}
                            />
                        </InputRightElement>
                    </InputGroup>
                    <FormErrorMessage>{errors.novaSenha?.message}</FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors.confirmarSenha}>
                    <FormLabel>Confirmar Nova Senha</FormLabel>
                    <InputGroup>
                        <InputLeftElement>
                            <FaLock color="gray.400" />
                        </InputLeftElement>
                        <Input
                            type={showPasswords.confirmar ? 'text' : 'password'}
                            placeholder="Confirme a nova senha"
                            {...register('confirmarSenha')}
                        />
                        <InputRightElement>
                            <IconButton
                                variant="ghost"
                                size="sm"
                                aria-label={showPasswords.confirmar ? 'Ocultar senha' : 'Mostrar senha'}
                                icon={showPasswords.confirmar ? <FaEyeSlash /> : <FaEye />}
                                onClick={() => togglePasswordVisibility('confirmar')}
                            />
                        </InputRightElement>
                    </InputGroup>
                    <FormErrorMessage>{errors.confirmarSenha?.message}</FormErrorMessage>
                </FormControl>

                <Button
                    type="submit"
                    colorScheme="blue"
                    leftIcon={<FaSave />}
                    isLoading={isSubmitting}
                    loadingText="Alterando..."
                    w="full"
                    size="lg"
                >
                    Alterar Senha
                </Button>
            </VStack>
        </form>
    );

    if (cardStyle) {
        return (
            <Card>
                <CardBody>
                    <FormContent />
                </CardBody>
            </Card>
        );
    }

    return <FormContent />;
}
