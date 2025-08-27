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
import { FaUserPlus, FaUser, FaEnvelope, FaLock, FaEye, FaEyeSlash } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { useNotification } from '../../hooks/useNotification';
import api from '../../services/api';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';


const schema = yup.object().shape({
  nome: yup.string().required('O nome é obrigatório').min(2, 'Nome deve ter pelo menos 2 caracteres'),
  email: yup.string().email('Digite um e-mail válido').required('O e-mail é obrigatório'),
  senha: yup.string()
    .required('A senha é obrigatória')
    .min(6, 'Senha deve ter pelo menos 6 caracteres')
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])/, 'Senha deve conter: 1 minúscula, 1 maiúscula, 1 número e 1 caractere especial'),
  confirmPassword: yup.string()
    .required('Confirmação de senha é obrigatória')
    .oneOf([yup.ref('senha'), null], 'As senhas não coincidem'),
});

const Registrar = () => {
  const [showPassword, setShowPassword] = React.useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = React.useState(false);
  const [shake, setShake] = React.useState(false);
  const { showSuccess, showError } = useNotification();
  const navigate = useNavigate();
  const bgGradient = useColorModeValue('linear(to-br, blue.400, purple.500)', 'linear(to-br, blue.700, purple.900)');

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: yupResolver(schema),
    mode: 'onTouched',
  });

  const onSubmit = async (data) => {
    try {
      const response = await api.post('/api/v1/Auth/registrar', {
        nome: data.nome,
        email: data.email,
        senha: data.senha,
      });
      if (response.data && response.data.success) {
        showSuccess('Cadastro realizado com sucesso!');
        reset();
        setTimeout(() => navigate('/login'), 1000);
      } else {
        showError(response.data?.message || 'Erro ao cadastrar usuário');
      }
    } catch (error) {
      const msg = error?.response?.data?.errors
        ? Object.values(error.response.data.errors).flat().join(', ')
        : error?.response?.data?.message || 'Erro ao cadastrar usuário';
      showError(msg);
      setShake(true);
      setTimeout(() => setShake(false), 500);
    }
  };

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
                <FaUserPlus color="#6366f1" size={32} />
              </Flex>
              <Heading as="h1" size="lg" color="gray.800">Crie sua conta</Heading>
              <Text color="gray.600" mt={2}>Preencha os campos abaixo</Text>
            </Box>
            <Box as="form" w="100%" onSubmit={handleSubmit(onSubmit)}>
              <VStack spacing={4}>
                <FormControl isInvalid={!!errors.nome}>
                  <FormLabel htmlFor="nome">Nome completo</FormLabel>
                  <InputGroup>
                    <InputLeftElement pointerEvents="none">
                      <FaUser color="gray.400" />
                    </InputLeftElement>
                    <Input
                      id="nome"
                      type="text"
                      placeholder="Seu nome"
                      {...register('nome')}
                    />
                  </InputGroup>
                  <FormErrorMessage>{errors.nome?.message}</FormErrorMessage>
                </FormControl>
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
                <FormControl isInvalid={!!errors.senha}>
                  <FormLabel htmlFor="senha">Senha</FormLabel>
                  <InputGroup>
                    <InputLeftElement pointerEvents="none">
                      <FaLock color="gray.400" />
                    </InputLeftElement>
                    <Input
                      id="senha"
                      type={showPassword ? 'text' : 'password'}
                      placeholder="••••••••"
                      {...register('senha')}
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
                  <FormErrorMessage>{errors.senha?.message}</FormErrorMessage>
                </FormControl>
                <FormControl isInvalid={!!errors.confirmPassword}>
                  <FormLabel htmlFor="confirmPassword">Confirmar senha</FormLabel>
                  <InputGroup>
                    <InputLeftElement pointerEvents="none">
                      <FaLock color="gray.400" />
                    </InputLeftElement>
                    <Input
                      id="confirmPassword"
                      type={showConfirmPassword ? 'text' : 'password'}
                      placeholder="••••••••"
                      {...register('confirmPassword')}
                    />
                    <InputRightElement>
                      <IconButton
                        variant="ghost"
                        aria-label={showConfirmPassword ? 'Ocultar senha' : 'Mostrar senha'}
                        icon={showConfirmPassword ? <FaEyeSlash /> : <FaEye />}
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        tabIndex={-1}
                      />
                    </InputRightElement>
                  </InputGroup>
                  <FormErrorMessage>{errors.confirmPassword?.message}</FormErrorMessage>
                </FormControl>
                <Button
                  type="submit"
                  colorScheme="blue"
                  size="lg"
                  width="full"
                  mt={4}
                  isLoading={isSubmitting}
                >
                  Cadastrar
                </Button>
              </VStack>
            </Box>
            <Box textAlign="center" mt={4}>
              <Text fontSize="sm" color="gray.600">
                Já tem uma conta?{' '}
                <Link color="blue.500" href="/login" _hover={{ color: 'blue.700', textDecoration: 'underline' }}>
                  Faça login
                </Link>
              </Text>
            </Box>
          </VStack>
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
      </Box>
    </Flex>
  );
};

export default Registrar;
