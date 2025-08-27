
import React from 'react';
import {
    Box,
    Button,
    Flex,
    FormControl,
    FormLabel,
    Input,
    InputGroup,
    InputLeftElement,
    InputRightElement,
    IconButton,
    Text,
    Heading,
    VStack,
    FormErrorMessage,
    useColorModeValue,
    Link,
} from '@chakra-ui/react';
import { FaUser, FaEnvelope, FaLock, FaEye, FaEyeSlash } from 'react-icons/fa';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';


const schema = yup.object().shape({
    email: yup.string().email('Email inválido').required('Email é obrigatório'),
    password: yup.string().required('Senha é obrigatória').min(6, 'A senha deve ter pelo menos 6 caracteres'),
});

const Login = () => {
    const { login } = useAuth();
    const { showError } = useNotification();
    const navigate = useNavigate();
    const [showPassword, setShowPassword] = React.useState(false);
    const [shake, setShake] = React.useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: yupResolver(schema),
        mode: 'onTouched',
    });

    const onSubmit = async (data) => {
        const result = await login(data.email, data.password);
        if (result.success) {
            navigate('/');
        } else {
            showError(result.message || 'Usuário ou senha inválidos');
            setShake(true);
            setTimeout(() => setShake(false), 500);
        }
    };

    const bgGradient = useColorModeValue('linear(to-br, blue.400, purple.500)', 'linear(to-br, blue.700, purple.900)');

    return (
        <Flex minH="100vh" align="center" justify="center" bgGradient={bgGradient} p={4}>
            <Box
                bg={useColorModeValue('white', 'gray.800')}
                rounded="2xl"
                shadow="2xl"
                w="full"
                maxW="md"
                transition="all 0.3s"
                className={shake ? 'shake' : ''}
                overflow="hidden"
            >
                <Box p={8}>
                    <VStack spacing={6}>
                        <Box textAlign="center" mb={2}>
                            <Flex w="20" h="20" bg="indigo.100" rounded="full" align="center" justify="center" mx="auto" mb={4}>
                                <FaUser color="#6366f1" size={32} />
                            </Flex>
                            <Heading as="h1" size="lg" color="gray.800">Bem-vindo de volta</Heading>
                            <Text color="gray.600" mt={2}>Entre na sua conta</Text>
                        </Box>
                        <Box as="form" w="100%" onSubmit={handleSubmit(onSubmit)}>
                            <VStack spacing={4}>
                                <FormControl isInvalid={!!errors.email}>
                                    <FormLabel htmlFor="email">Email</FormLabel>
                                    <InputGroup>
                                        <InputLeftElement pointerEvents="none">
                                            <FaEnvelope color="gray.400" />
                                        </InputLeftElement>
                                        <Input
                                            id="email"
                                            type="email"
                                            placeholder="seu@email.com"
                                            {...register('email')}
                                        />
                                    </InputGroup>
                                    <FormErrorMessage>{errors.email?.message}</FormErrorMessage>
                                </FormControl>
                                <FormControl isInvalid={!!errors.password}>
                                    <FormLabel htmlFor="password">Senha</FormLabel>
                                    <InputGroup>
                                        <InputLeftElement pointerEvents="none">
                                            <FaLock color="gray.400" />
                                        </InputLeftElement>
                                        <Input
                                            id="password"
                                            type={showPassword ? 'text' : 'password'}
                                            placeholder="••••••••"
                                            {...register('password')}
                                        />
                                        <InputRightElement>
                                            <IconButton
                                                variant="ghost"
                                                aria-label={showPassword ? 'Ocultar senha' : 'Mostrar senha'}
                                                icon={showPassword ? <FaEyeSlash /> : <FaEye />}
                                                onClick={() => setShowPassword(!showPassword)}
                                                tabIndex={-1}
                                            />
                                        </InputRightElement>
                                    </InputGroup>
                                    <FormErrorMessage>{errors.password?.message}</FormErrorMessage>
                                </FormControl>

                                <Button
                                    type="submit"
                                    colorScheme="blue"
                                    size="lg"
                                    width="full"
                                    mt={4}
                                    isLoading={isSubmitting}
                                >
                                    Entrar
                                </Button>

                            </VStack>
                        </Box>
                        <Box textAlign="center" mt={4}>
                            <Text fontSize="sm" color="gray.600">
                                Não tem uma conta?{' '}
                                <Link color="blue.500" as="button" onClick={() => navigate('/registrar')} _hover={{ color: 'blue.700', textDecoration: 'underline' }}>
                                    Cadastre-se
                                </Link>
                            </Text>
                        </Box>
                    </VStack>
                </Box>
            </Box>
            <style>{`
                .shake {
                  animation: shake 0.5s cubic-bezier(.36,.07,.19,.97) both;
                }
                @keyframes shake {
                  0%, 100% { transform: translateX(0); }
                  10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
                  20%, 40%, 60%, 80% { transform: translateX(5px); }
                }
            `}</style>
        </Flex>
    );
};

export default Login;
