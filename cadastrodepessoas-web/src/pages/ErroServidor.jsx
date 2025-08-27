import React from 'react';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    VStack,
    Button,
    Heading,
    Container,
} from '@chakra-ui/react';
import {
    FaExclamationTriangle,
    FaHome,
    FaRedo,
} from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';

export default function ErroServidor() {
    const bgGradient = useColorModeValue('linear(to-br, blue.400, purple.500)', 'linear(to-br, blue.700, purple.900)');
    const bgCard = useColorModeValue('white', 'gray.800');
    const navigate = useNavigate();

    const handleVoltar = () => {
        navigate('/');
    };

    const handleRecarregar = () => {
        window.location.reload();
    };

    return (
        <Flex minH="100vh" align="center" justify="center" bgGradient={bgGradient} p={4}>
            <Container maxW="md">
                <Box
                    bg={bgCard}
                    rounded="2xl"
                    shadow="2xl"
                    overflow="hidden"
                    transition="all 0.3s"
                    textAlign="center"
                >
                    <Box p={8}>
                        <VStack spacing={6}>
                            <Heading 
                                as="h1" 
                                size="4xl" 
                                color="blue.400"
                                fontWeight="bold"
                                mb={4}
                            >
                                500
                            </Heading>

                            <Flex 
                                w="20" 
                                h="20" 
                                bg="gray.100" 
                                rounded="full" 
                                align="center" 
                                justify="center" 
                                mx="auto" 
                                mb={4}
                                border="4px solid"
                                borderColor="gray.800"
                            >
                                <FaExclamationTriangle color="#2d3748" size={32} />
                            </Flex>

                            <Heading as="h2" size="lg" color="gray.800" mb={2}>
                                Erro do Servidor
                            </Heading>

                            <Text color="gray.600" mb={6} px={4}>
                                Erro interno ao buscar pessoa. Nossa equipe foi notificada e está 
                                trabalhando para resolver o problema. Por favor, tente novamente mais tarde.
                            </Text>

                            <VStack w="100%" spacing={3}>
                                <Button
                                    leftIcon={<FaHome />}
                                    colorScheme="blue"
                                    size="lg"
                                    width="full"
                                    onClick={handleVoltar}
                                >
                                    Voltar para o Início
                                </Button>

                                <Button
                                    variant="outline"
                                    size="lg"
                                    width="full"
                                    onClick={handleRecarregar}
                                    color="blue.500"
                                    borderColor="blue.500"
                                    _hover={{ bg: 'blue.50' }}
                                >
                                    Voltar
                                </Button>
                            </VStack>
                        </VStack>
                    </Box>
                </Box>
            </Container>
        </Flex>
    );
}
