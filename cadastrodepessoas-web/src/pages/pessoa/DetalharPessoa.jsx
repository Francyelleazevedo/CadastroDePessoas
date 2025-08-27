import React, { useState, useEffect } from 'react';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    VStack,
    HStack,
    Button,
    Spinner,
    Alert,
    AlertIcon,
    AlertTitle,
    AlertDescription,
    Card,
    CardBody,
    Divider,
    Badge,
    Grid,
    GridItem,
} from '@chakra-ui/react';
import {
    FaArrowLeft,
    FaEdit,
    FaUser,
    FaEnvelope,
    FaIdCard,
    FaCalendarAlt,
    FaMapMarkerAlt,
    FaPhone,
    FaVenusMars,
    FaBirthdayCake,
} from 'react-icons/fa';
import { useNavigate, useParams } from 'react-router-dom';
import Header from '../../components/layout/Header';
import SideBar from '../../components/layout/SideBar';
import { pessoaService } from '../../services/pessoaService';
import { useNotification } from '../../hooks/useNotification';
import { calcularIdade } from '../../utils/dateUtils';
import { maskCPF } from '../../utils/masks';
import { formatarCEP, formatarData } from '../../utils/formatters';

export default function DetalharPessoa() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const bgCard = useColorModeValue('white', 'gray.800');
    const navigate = useNavigate();
    const { showError } = useNotification();
    const { id } = useParams();

    const [isLoading, setIsLoading] = useState(true);
    const [pessoa, setPessoa] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        const carregarPessoa = async () => {
            try {
                setIsLoading(true);
                const data = await pessoaService.obterPorId(id);
                setPessoa(data);
                setError(null);
            } catch (error) {
                console.error('Erro ao carregar pessoa:', error);
                setError('Não foi possível carregar os dados da pessoa');
                showError('Não foi possível carregar os dados da pessoa');
            } finally {
                setIsLoading(false);
            }
        };

        if (id) {
            carregarPessoa();
        }
    }, [id, showError]);

    const handleVoltar = () => {
        navigate('/pessoas');
    };

    const handleEditar = () => {
        navigate(`/pessoas/editar/${id}`);
    };

    if (isLoading) {
        return (
            <Flex>
                <SideBar />
                <Flex direction="column" flex={1}>
                    <Header />
                    <Box as="main" p={6} bg={bgMain}>
                        <VStack align="center" spacing={4} mt={20}>
                            <Spinner size="xl" color="blue.500" />
                            <Text>Carregando dados da pessoa...</Text>
                        </VStack>
                    </Box>
                </Flex>
            </Flex>
        );
    }

    if (error) {
        return (
            <Flex>
                <SideBar />
                <Flex direction="column" flex={1}>
                    <Header />
                    <Box as="main" p={6} bg={bgMain}>
                        <VStack align="stretch" spacing={6} maxW="800px" mx="auto">
                            <Alert status="error">
                                <AlertIcon />
                                <Box>
                                    <AlertTitle>Erro!</AlertTitle>
                                    <AlertDescription>{error}</AlertDescription>
                                </Box>
                            </Alert>
                            <Button
                                leftIcon={<FaArrowLeft />}
                                variant="outline"
                                onClick={handleVoltar}
                                alignSelf="flex-start"
                            >
                                Voltar
                            </Button>
                        </VStack>
                    </Box>
                </Flex>
            </Flex>
        );
    }

    const idade = calcularIdade(pessoa?.DataNascimento);

    return (
        <Flex>
            <SideBar />
            <Flex direction="column" flex={1}>
                <Header />
                <Box as="main" p={6} bg={bgMain}>
                    <VStack align="stretch" spacing={6} maxW="1000px" mx="auto">
                        <Flex justify="space-between" align="center">
                            <Box>
                                <Text fontSize="2xl" fontWeight="bold" color="gray.700">
                                    Detalhes da Pessoa
                                </Text>
                                <Text color="gray.500">
                                    Visualize todas as informações de {pessoa?.Nome}
                                </Text>
                            </Box>
                            <HStack spacing={3}>
                                <Button
                                    leftIcon={<FaEdit />}
                                    colorScheme="blue"
                                    onClick={handleEditar}
                                >
                                    Editar
                                </Button>
                                <Button
                                    leftIcon={<FaArrowLeft />}
                                    variant="outline"
                                    onClick={handleVoltar}
                                >
                                    Voltar
                                </Button>
                            </HStack>
                        </Flex>

                        <Grid templateColumns={{ base: "1fr", lg: "2fr 1fr" }} gap={6}>
                            <GridItem>
                                <Card bg={bgCard} shadow="md">
                                    <CardBody>
                                        <VStack align="stretch" spacing={4}>
                                            <Flex align="center" gap={3}>
                                                <Box p={2} bg="blue.100" rounded="lg">
                                                    <FaUser color="#3182ce" size={20} />
                                                </Box>
                                                <Text fontSize="lg" fontWeight="bold" color="gray.700">
                                                    Informações Pessoais
                                                </Text>
                                            </Flex>
                                            <Divider />

                                            <HStack spacing={4}>
                                                <FaUser color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        Nome Completo
                                                    </Text>
                                                    <Text fontWeight="medium">
                                                        {pessoa?.Nome || 'N/A'}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaIdCard color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        CPF
                                                    </Text>
                                                    <Text fontFamily="mono">
                                                        {pessoa?.Cpf ? maskCPF(pessoa.Cpf) : 'N/A'}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaEnvelope color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        E-mail
                                                    </Text>
                                                    <Text>
                                                        {pessoa?.Email || 'N/A'}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaBirthdayCake color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        Data de Nascimento
                                                    </Text>
                                                    <Text>
                                                        {pessoa?.DataNascimento ? formatarData(pessoa.DataNascimento) : 'N/A'}
                                                        {idade !== null && (
                                                            <Badge ml={2} colorScheme="blue" variant="subtle">
                                                                {idade} anos
                                                            </Badge>
                                                        )}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaVenusMars color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        Sexo
                                                    </Text>
                                                    <Text>
                                                        {(() => {
                                                            switch (Number(pessoa?.Sexo)) {
                                                                case 1: return 'Masculino';
                                                                case 2: return 'Feminino';
                                                                case 3: return 'Outro';
                                                                case 0:
                                                                default: return 'N/A';
                                                            }
                                                        })()}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaMapMarkerAlt color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        Naturalidade
                                                    </Text>
                                                    <Text>
                                                        {pessoa?.Naturalidade || 'N/A'}
                                                    </Text>
                                                </Box>
                                            </HStack>

                                            <HStack spacing={4}>
                                                <FaPhone color="gray" />
                                                <Box>
                                                    <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                        Telefone
                                                    </Text>
                                                    <Text>
                                                        {pessoa?.Telefone || 'N/A'}
                                                    </Text>
                                                </Box>
                                            </HStack>
                                        </VStack>
                                    </CardBody>
                                </Card>
                            </GridItem>

                            <GridItem>
                                <Card bg={bgCard} shadow="md">
                                    <CardBody>
                                        <VStack align="stretch" spacing={4}>
                                            <Flex align="center" gap={3}>
                                                <Box p={2} bg="green.100" rounded="lg">
                                                    <FaMapMarkerAlt color="#38a169" size={20} />
                                                </Box>
                                                <Text fontSize="lg" fontWeight="bold" color="gray.700">
                                                    Endereço
                                                </Text>
                                            </Flex>
                                            <Divider />

                                            {pessoa?.Endereco ? (
                                                <VStack align="stretch" spacing={3}>
                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Logradouro
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Logradouro || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Número
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Numero || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Complemento
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Complemento || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Bairro
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Bairro || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Cidade
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Cidade || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            Estado
                                                        </Text>
                                                        <Text>
                                                            {pessoa.Endereco.Estado || 'N/A'}
                                                        </Text>
                                                    </Box>

                                                    <Box>
                                                        <Text fontSize="sm" color="gray.500" fontWeight="medium">
                                                            CEP
                                                        </Text>
                                                        <Text fontFamily="mono">
                                                            {pessoa.Endereco.Cep ? formatCEP(pessoa.Endereco.Cep) : 'N/A'}
                                                        </Text>
                                                    </Box>
                                                </VStack>
                                            ) : (
                                                <Text color="gray.500" textAlign="center" py={4}>
                                                    Endereço não informado
                                                </Text>
                                            )}
                                        </VStack>
                                    </CardBody>
                                </Card>
                            </GridItem>
                        </Grid>
                    </VStack>
                </Box>
            </Flex>
        </Flex>
    );
}
