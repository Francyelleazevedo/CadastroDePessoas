import React, { useState, useEffect } from 'react';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    VStack,
    Button,
    Spinner,
    Alert,
    AlertIcon,
    AlertTitle,
    AlertDescription,
    useDisclosure,
} from '@chakra-ui/react';
import {
    FaArrowLeft,
} from 'react-icons/fa';
import { useNavigate, useParams } from 'react-router-dom';
import Header from '../../components/layout/Header';
import SideBar from '../../components/layout/SideBar';
import PessoaForm from '../../components/pessoa/PessoaForm';
import { pessoaService } from '../../services/pessoaService';
import { useNotification } from '../../hooks/useNotification';

export default function EditarPessoa() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();
    const { id } = useParams();
    const { isOpen, onOpen, onClose } = useDisclosure();

    const [isLoading, setIsLoading] = useState(false);
    const [isLoadingData, setIsLoadingData] = useState(true);
    const [pessoa, setPessoa] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        const carregarPessoa = async () => {
            try {
                setIsLoadingData(true);
                const data = await pessoaService.obterPorId(id);
                setPessoa(data);
                setError(null);
            } catch (error) {
                setError('Não foi possível carregar os dados da pessoa');
            } finally {
                setIsLoadingData(false);
            }
        };

        if (id) {
            carregarPessoa();
        }
    }, [id]);

    const handleSubmit = async (pessoaData) => {
        setIsLoading(true);
        try {
            const dadosCompletos = {
                ...pessoaData
            };

            await pessoaService.atualizar(dadosCompletos);

            showSuccess('Pessoa atualizada com sucesso!');
            navigate('/pessoas');
        } catch (error) {
            showError('Erro ao atualizar pessoa.');
            throw error;
        } finally {
            setIsLoading(false);
        }
    };

    const handleCancel = () => {
        navigate('/pessoas');
    };

    if (isLoadingData) {
        return (
            <Flex>
                <SideBar isOpen={isOpen} onClose={onClose} />
                <Flex direction="column" flex={1}>
                    <Header onOpenSidebar={onOpen} />
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
                <SideBar isOpen={isOpen} onClose={onClose} />
                <Flex direction="column" flex={1}>
                    <Header onOpenSidebar={onOpen} />
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
                                onClick={handleCancel}
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

    return (
        <Flex>
            <SideBar isOpen={isOpen} onClose={onClose} />
            <Flex direction="column" flex={1}>
                <Header onOpenSidebar={onOpen} />
                <Box as="main" p={6} bg={bgMain}>
                    <VStack align="stretch" spacing={6} maxW="800px" mx="auto">
                        {/* Cabeçalho da página */}
                        <Flex justify="space-between" align="center">
                            <Box>
                                <Text fontSize="2xl" fontWeight="bold" color="gray.700">
                                    Editar Pessoa
                                </Text>
                                <Text color="gray.500">
                                    Atualize os dados da pessoa {pessoa?.Nome}
                                </Text>
                            </Box>
                            <Button
                                leftIcon={<FaArrowLeft />}
                                variant="outline"
                                onClick={handleCancel}
                            >
                                Voltar
                            </Button>
                        </Flex>

                        <PessoaForm
                            initialData={pessoa}
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={isLoading}
                            submitButtonText="Atualizar Pessoa"
                            title="Dados Pessoais"
                        />
                    </VStack>
                </Box>
            </Flex>
        </Flex>
    );
}
