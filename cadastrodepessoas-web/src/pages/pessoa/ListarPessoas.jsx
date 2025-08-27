import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Flex,
    Text,
    useColorModeValue,
    Table,
    Thead,
    Tbody,
    Tr,
    Th,
    Td,
    TableContainer,
    Badge,
    IconButton,
    HStack,
    VStack,
    Spinner,
    Alert,
    AlertIcon,
    AlertTitle,
    AlertDescription,
    Button,
    InputGroup,
    InputLeftElement,
    Input,
    Select,
    useDisclosure,
} from '@chakra-ui/react';
import {
    FaEdit,
    FaTrash,
    FaPlus,
    FaSearch,
    FaFilter,
    FaEye,
} from 'react-icons/fa';
import Header from '../../components/layout/Header';
import SideBar from '../../components/layout/SideBar';
import ConfirmarExclusaoModal from '../../components/modals/ConfirmarExclusaoModal';
import { pessoaService } from '../../services/pessoaService';
import { calcularIdade } from '../../utils/dateUtils';
import { maskCPF } from '../../utils/masks';
import { useNotification } from '../../hooks/useNotification';

export default function ListarPessoas() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const bgCard = useColorModeValue('white', 'gray.800');
    const navigate = useNavigate();
    const [pessoas, setPessoas] = React.useState([]);
    const [loading, setLoading] = React.useState(true);
    const [error, setError] = React.useState(null);
    const [searchTerm, setSearchTerm] = React.useState('');
    const [sexoFilter, setSexoFilter] = React.useState('');
    const [pessoaParaExcluir, setPessoaParaExcluir] = React.useState(null);
    const [loadingExclusao, setLoadingExclusao] = React.useState(false);
    const { showSuccess, showError } = useNotification();
    const { isOpen, onOpen, onClose } = useDisclosure();

    const fetchPessoas = React.useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await pessoaService.listar();
            setPessoas(data);
        } catch (err) {
            setError('Erro ao carregar pessoas');
            showError('Não foi possível carregar a lista de pessoas');
        } finally {
            setLoading(false);
        }
    }, [showError]);

    React.useEffect(() => {
        fetchPessoas();
    }, [fetchPessoas]);

    const pessoasFiltradas = pessoas.filter(pessoa => {
        const search = searchTerm.trim().toLowerCase();
        const searchCpf = search.replace(/[^\d]/g, '');
        const matchNome = pessoa.Nome?.toLowerCase().includes(search) || false;
        const matchCpf = pessoa.Cpf?.replace(/[^\d]/g, '').includes(searchCpf) || false;
        const matchSexo = sexoFilter === '' || pessoa.Sexo?.toString() === sexoFilter;

        return (matchNome || matchCpf) && matchSexo;
    });

    const handleOpenDeleteModal = (pessoa) => {
        setPessoaParaExcluir(pessoa);
        onOpen();
    };

    const handleCloseDeleteModal = () => {
        onClose();
        setPessoaParaExcluir(null);
        setLoadingExclusao(false);
    };

    const handleConfirmDelete = async (id, nome) => {
        setLoadingExclusao(true);
        try {
            await pessoaService.deletar(id);
            showSuccess(`${nome} foi excluída com sucesso`);
            fetchPessoas(); 
            handleCloseDeleteModal(); 
        } catch (err) {
            console.error('Erro ao excluir pessoa:', err);
            showError('Não foi possível excluir a pessoa. Tente novamente.');
            setLoadingExclusao(false);
        }
    };

    return (
        <Flex h="100vh" overflow="hidden">
            <SideBar />
            <Flex direction="column" flex={1} overflow="hidden">
                <Header />
                <Box as="main" flex={1} overflowY="auto" p={6} bg={bgMain}>
                    <VStack align="stretch" spacing={6}>
                        <Flex justify="space-between" align="center">
                            <Box>
                                <Text fontSize="2xl" fontWeight="bold" color="gray.700">
                                    Listagem de Pessoas
                                </Text>
                                <Text color="gray.500">
                                    Gerencie todas as pessoas cadastradas no sistema
                                </Text>
                            </Box>
                            <Button
                                leftIcon={<FaPlus />}
                                colorScheme="blue"
                                onClick={() => navigate('/pessoas/criar')}
                            >
                                Nova Pessoa
                            </Button>
                        </Flex>

                        <Box bg={bgCard} p={4} rounded="lg" shadow="md">
                            <HStack spacing={4}>
                                <InputGroup flex={1}>
                                    <InputLeftElement>
                                        <FaSearch color="gray.400" />
                                    </InputLeftElement>
                                    <Input
                                        placeholder="Buscar por nome, email ou CPF..."
                                        value={searchTerm}
                                        onChange={(e) => setSearchTerm(e.target.value)}
                                    />
                                </InputGroup>
                                <Select
                                    placeholder="Filtrar por sexo"
                                    w="200px"
                                    value={sexoFilter}
                                    onChange={(e) => setSexoFilter(e.target.value)}
                                >
                                    <option value="1">Masculino</option>
                                    <option value="2">Feminino</option>
                                    <option value="0">Não informado</option>
                                </Select>
                            </HStack>
                        </Box>

                        <Box bg={bgCard} rounded="lg" shadow="md" overflow="hidden">
                            {loading ? (
                                <Flex justify="center" align="center" h="200px">
                                    <VStack>
                                        <Spinner size="xl" color="blue.500" />
                                        <Text>Carregando pessoas...</Text>
                                    </VStack>
                                </Flex>
                            ) : error ? (
                                <Alert status="error" m={4}>
                                    <AlertIcon />
                                    <Box>
                                        <AlertTitle>Erro!</AlertTitle>
                                        <AlertDescription>{error}</AlertDescription>
                                    </Box>
                                </Alert>
                            ) : pessoasFiltradas.length === 0 ? (
                                <Flex justify="center" align="center" h="200px">
                                    <VStack>
                                        <Text fontSize="lg" color="gray.500">
                                            {pessoas.length === 0
                                                ? 'Nenhuma pessoa cadastrada'
                                                : 'Nenhuma pessoa encontrada com os filtros aplicados'
                                            }
                                        </Text>
                                        {pessoas.length === 0 && (
                                            <Button
                                                leftIcon={<FaPlus />}
                                                colorScheme="blue"
                                                onClick={() => navigate('/pessoas/criar')}
                                            >
                                                Cadastrar Pessoa
                                            </Button>
                                        )}
                                    </VStack>
                                </Flex>
                            ) : (
                                <>
                                    <TableContainer>
                                        <Table variant="simple">
                                            <Thead bg="gray.50">
                                                <Tr>
                                                    <Th>Nome</Th>
                                                    <Th>CPF</Th>
                                                    <Th>Idade</Th>
                                                    <Th>Sexo</Th>
                                                    <Th width="120px">Ações</Th>
                                                </Tr>
                                            </Thead>
                                            <Tbody>
                                                {pessoasFiltradas.map((pessoa) => {
                                                    const idade = calcularIdade(pessoa.DataNascimento);
                                                    return (
                                                        <Tr key={pessoa.Id}>
                                                            <Td>
                                                                <Text fontWeight="medium">
                                                                    {pessoa.Nome || 'N/A'}
                                                                </Text>
                                                            </Td>
                                                            <Td>
                                                                <Text fontFamily="mono" color="gray.600">
                                                                    {pessoa.Cpf ? maskCPF(pessoa.Cpf) : 'N/A'}
                                                                </Text>
                                                            </Td>
                                                            <Td>
                                                                <Text>
                                                                    {idade !== null ? `${idade} anos` : 'N/A'}
                                                                </Text>
                                                            </Td>
                                                            <Td>
                                                                <Text>
                                                                    {(() => {
                                                                        switch (Number(pessoa.Sexo)) {
                                                                            case 1: return 'Masculino';
                                                                            case 2: return 'Feminino';
                                                                            case 3: return 'Outro';
                                                                            case 0:
                                                                            default: return 'N/A';
                                                                        }
                                                                    })()}
                                                                </Text>
                                                            </Td>
                                                            <Td>
                                                                <HStack spacing={1}>
                                                                    <IconButton
                                                                        aria-label="Detalhes"
                                                                        icon={<FaEye />}
                                                                        size="sm"
                                                                        colorScheme="green"
                                                                        variant="ghost"
                                                                        onClick={() => navigate(`/pessoas/detalhes/${pessoa.Id}`)}
                                                                    />
                                                                    <IconButton
                                                                        aria-label="Editar"
                                                                        icon={<FaEdit />}
                                                                        size="sm"
                                                                        colorScheme="blue"
                                                                        variant="ghost"
                                                                        onClick={() => navigate(`/pessoas/editar/${pessoa.Id}`)}
                                                                    />
                                                                    <IconButton
                                                                        aria-label="Excluir"
                                                                        icon={<FaTrash />}
                                                                        size="sm"
                                                                        colorScheme="red"
                                                                        variant="ghost"
                                                                        onClick={() => handleOpenDeleteModal(pessoa)}
                                                                    />
                                                                </HStack>
                                                            </Td>
                                                        </Tr>
                                                    );
                                                })}
                                            </Tbody>
                                        </Table>
                                    </TableContainer>

                                    <Box p={4} borderTop="1px" borderColor="gray.200">
                                        <Text fontSize="sm" color="gray.600">
                                            Mostrando {pessoasFiltradas.length} de {pessoas.length} pessoas
                                        </Text>
                                    </Box>
                                </>
                            )}
                        </Box>
                    </VStack>
                </Box>
            </Flex>

            {/* Modal de Confirmação de Exclusão */}
            <ConfirmarExclusaoModal
                isOpen={isOpen}
                onClose={handleCloseDeleteModal}
                onConfirm={handleConfirmDelete}
                pessoa={pessoaParaExcluir}
                isLoading={loadingExclusao}
            />
        </Flex>
    );
}
