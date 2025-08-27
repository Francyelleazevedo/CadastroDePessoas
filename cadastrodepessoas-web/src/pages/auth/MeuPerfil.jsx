import React, { useState, useEffect } from 'react';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    VStack,
    HStack,
    Button,
    Avatar,
    Card,
    CardHeader,
    CardBody,
    Heading,
    Badge,
    Divider,
    Spinner,
    Alert,
    AlertIcon,
    SimpleGrid,
    useDisclosure,
} from '@chakra-ui/react';
import {
    FaUser,
    FaEnvelope,
    FaCalendarAlt,
    FaKey,
    FaArrowLeft,
} from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header';
import SideBar from '../../components/layout/SideBar';
import AlterarSenhaForm from '../../components/forms/AlterarSenhaForm';
import { perfilService } from '../../services/perfilService';
import { formatarData } from '../../utils/formatters';
import { useNotification } from '../../hooks/useNotification';

export default function MeuPerfil() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const bgCard = useColorModeValue('white', 'gray.800');
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();
    const { isOpen, onOpen, onClose } = useDisclosure();
    
    const [usuario, setUsuario] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [activeTab, setActiveTab] = useState('perfil');

    useEffect(() => {
        const carregarPerfil = async () => {
            try {
                setLoading(true);
                const response = await perfilService.obterPerfil();
                setUsuario(response.user);
                setError(null);
            } catch (error) {
                console.error('Erro ao carregar perfil:', error);
                setError('Não foi possível carregar as informações do perfil');
                showError('Não foi possível carregar as informações do perfil');
            } finally {
                setLoading(false);
            }
        };

        carregarPerfil();
    }, [showError]);

    const handlePasswordSuccess = () => {
        showSuccess('Senha alterada com sucesso! Por segurança, faça login novamente.');
    };

    const getInitials = (nome) => {
        if (!nome) return 'U';
        return nome.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
    };

    if (loading) {
        return (
            <Flex>
                <SideBar isOpen={isOpen} onClose={onClose} />
                <Flex direction="column" flex={1}>
                    <Header onOpenSidebar={onOpen} />
                    <Box as="main" p={6} bg={bgMain}>
                        <VStack align="center" spacing={4} mt={20}>
                            <Spinner size="xl" color="blue.500" />
                            <Text>Carregando perfil...</Text>
                        </VStack>
                    </Box>
                </Flex>
            </Flex>
        );
    }

    if (error) {
        return (
            <Flex>
                <SideBar isOpen={isOpen} onClose={onClose} />
                <Flex direction="column" flex={1}>
                    <Header onOpenSidebar={onOpen} />
                    <Box as="main" p={6} bg={bgMain}>
                        <VStack align="stretch" spacing={6} maxW="800px" mx="auto">
                            <Alert status="error">
                                <AlertIcon />
                                {error}
                            </Alert>
                            <Button
                                leftIcon={<FaArrowLeft />}
                                variant="outline"
                                onClick={() => navigate('/')}
                                alignSelf="flex-start"
                            >
                                Voltar ao Dashboard
                            </Button>
                        </VStack>
                    </Box>
                </Flex>
            </Flex>
        );
    }

    return (
        <Flex>
            <SideBar isOpen={isOpen} onClose={onClose} />
            <Flex direction="column" flex={1}>
                <Header onOpenSidebar={onOpen} />
                <Box as="main" p={6} bg={bgMain}>
                    <VStack align="stretch" spacing={6} maxW="800px" mx="auto">
                        {/* Cabeçalho */}
                        <Flex justify="space-between" align="center">
                            <Box>
                                <Text fontSize="2xl" fontWeight="bold" color="gray.700">
                                    Meu Perfil
                                </Text>
                                <Text color="gray.500">
                                    Gerencie suas informações pessoais e configurações
                                </Text>
                            </Box>
                            <Button
                                leftIcon={<FaArrowLeft />}
                                variant="outline"
                                onClick={() => navigate('/')}
                            >
                                Voltar
                            </Button>
                        </Flex>

                        {/* Tabs */}
                        <HStack spacing={4} bg={bgCard} p={1} borderRadius="lg">
                            <Button
                                variant={activeTab === 'perfil' ? 'solid' : 'ghost'}
                                colorScheme={activeTab === 'perfil' ? 'blue' : 'gray'}
                                onClick={() => setActiveTab('perfil')}
                                leftIcon={<FaUser />}
                                flex={1}
                            >
                                Informações
                            </Button>
                            <Button
                                variant={activeTab === 'senha' ? 'solid' : 'ghost'}
                                colorScheme={activeTab === 'senha' ? 'blue' : 'gray'}
                                onClick={() => setActiveTab('senha')}
                                leftIcon={<FaKey />}
                                flex={1}
                            >
                                Alterar Senha
                            </Button>
                        </HStack>

                        {/* Conteúdo das Tabs */}
                        {activeTab === 'perfil' && (
                            <Card bg={bgCard}>
                                <CardHeader>
                                    <Heading size="md">Informações Pessoais</Heading>
                                </CardHeader>
                                <CardBody>
                                    <VStack spacing={6}>
                                        {/* Avatar e Info Principal */}
                                        <HStack spacing={6} align="start" w="full">
                                            <Avatar
                                                size="xl"
                                                name={usuario?.Nome}
                                                bg="blue.500"
                                                color="white"
                                            >
                                                {getInitials(usuario?.Nome)}
                                            </Avatar>
                                            <VStack align="start" spacing={2} flex={1}>
                                                <Heading size="lg" color="gray.700">
                                                    {usuario?.Nome}
                                                </Heading>
                                                <HStack>
                                                    <Badge colorScheme="green">Ativo</Badge>
                                                    <Badge colorScheme="blue">Usuário</Badge>
                                                </HStack>
                                                <Text color="gray.500" fontSize="sm">
                                                    Membro desde {formatarData(usuario?.DataCadastro)}
                                                </Text>
                                            </VStack>
                                        </HStack>

                                        <Divider />

                                        {/* Detalhes */}
                                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6} w="full">
                                            <VStack align="start" spacing={3}>
                                                <HStack>
                                                    <Box color="gray.500">
                                                        <FaUser />
                                                    </Box>
                                                    <Text fontWeight="medium">Nome</Text>
                                                </HStack>
                                                <Text pl={6} color="gray.600">
                                                    {usuario?.Nome}
                                                </Text>
                                            </VStack>

                                            <VStack align="start" spacing={3}>
                                                <HStack>
                                                    <Box color="gray.500">
                                                        <FaEnvelope />
                                                    </Box>
                                                    <Text fontWeight="medium">Email</Text>
                                                </HStack>
                                                <Text pl={6} color="gray.600">
                                                    {usuario?.Email}
                                                </Text>
                                            </VStack>

                                            <VStack align="start" spacing={3}>
                                                <HStack>
                                                    <Box color="gray.500">
                                                        <FaCalendarAlt />
                                                    </Box>
                                                    <Text fontWeight="medium">Data de Cadastro</Text>
                                                </HStack>
                                                <Text pl={6} color="gray.600">
                                                    {formatarData(usuario?.DataCadastro)}
                                                </Text>
                                            </VStack>

                                            <VStack align="start" spacing={3}>
                                                <HStack>
                                                    <Box color="gray.500">
                                                        <FaUser />
                                                    </Box>
                                                    <Text fontWeight="medium">ID do Usuário</Text>
                                                </HStack>
                                                <Text pl={6} color="gray.600" fontSize="sm" fontFamily="mono">
                                                    {usuario?.Id}
                                                </Text>
                                            </VStack>
                                        </SimpleGrid>
                                    </VStack>
                                </CardBody>
                            </Card>
                        )}

                        {activeTab === 'senha' && (
                            <AlterarSenhaForm 
                                onSuccess={handlePasswordSuccess}
                                cardStyle={true}
                            />
                        )}
                    </VStack>
                </Box>
            </Flex>
        </Flex>
    );
}
