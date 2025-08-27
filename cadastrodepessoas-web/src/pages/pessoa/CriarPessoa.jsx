import React, { useState } from 'react';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    VStack,
    Button,
    useDisclosure,
} from '@chakra-ui/react';
import {
    FaArrowLeft,
} from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header';
import SideBar from '../../components/layout/SideBar';
import PessoaForm from '../../components/pessoa/PessoaForm';

import { pessoaService } from '../../services/pessoaService';
import { useNotification } from '../../hooks/useNotification';

export default function CriarPessoa() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();
    const [isLoading, setIsLoading] = useState(false);
    const { isOpen, onOpen, onClose } = useDisclosure();

    const handleSubmit = async (pessoaData) => {
        setIsLoading(true);
        try {
            await pessoaService.criar(pessoaData);
            showSuccess('Pessoa cadastrada com sucesso!');
            navigate('/pessoas');
        } catch (error) {
            showError('Erro ao cadastrar pessoa.');
            throw error;
        } finally {
            setIsLoading(false);
        }
    };

    const handleCancel = () => {
        navigate('/pessoas');
    };

    return (
        <Flex>
            <SideBar isOpen={isOpen} onClose={onClose} />
            <Flex direction="column" flex={1}>
                <Header onOpenSidebar={onOpen} />
                <Box as="main" p={6} bg={bgMain}>
                    <VStack align="stretch" spacing={6} maxW="800px" mx="auto">
                        <Flex justify="space-between" align="center">
                            <Box>
                                <Text fontSize="2xl" fontWeight="bold" color="gray.700">
                                    Cadastrar Nova Pessoa
                                </Text>
                                <Text color="gray.500">
                                    Preencha os dados abaixo para cadastrar uma nova pessoa
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
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={isLoading}
                            submitButtonText="Salvar Pessoa"
                            title="Dados Pessoais"
                        />
                    </VStack>
                </Box>
            </Flex>
        </Flex>
    );
}
