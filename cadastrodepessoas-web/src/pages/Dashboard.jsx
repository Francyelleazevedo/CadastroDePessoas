import React from 'react';
import {
    Box,
    Flex,
    Grid,
    Text,
    useColorModeValue,
    VStack,
    HStack,
    useDisclosure,
} from '@chakra-ui/react';
import Header from '../components/layout/Header';
import SideBar from '../components/layout/SideBar';
import { FaUsers, FaUserPlus, FaChartPie, FaCheckCircle, FaEnvelope, FaShoppingCart } from 'react-icons/fa';
import { Pie } from 'react-chartjs-2';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { pessoaService } from '../services/pessoaService';
import { useDashboardAutoRefresh } from '../hooks/useDashboardAutoRefresh';
import { calcularIdade } from '../utils/dateUtils';

ChartJS.register(ArcElement, Tooltip, Legend);

const initialStats = {
    totalPessoas: 0,
    novosCadastros: 0,
    idadeMedia: 0,
    sexo: {
        masculino: 0,
        feminino: 0,
        naoInformado: 0,
    },
};

export default function Dashboard() {
    const bgMain = useColorModeValue('gray.100', 'gray.900');
    const [stats, setStats] = React.useState(initialStats);
    const [loading, setLoading] = React.useState(true);
    const { isOpen, onOpen, onClose } = useDisclosure();

    // Busca lista de pessoas e calcula os dados do dashboard
    const fetchStats = React.useCallback(async () => {
        setLoading(true);
        try {
            const pessoas = await pessoaService.listar();
            // Total de pessoas
            const totalPessoas = pessoas.length;
            // Novos cadastros nos últimos 30 dias
            const trintaDiasAtras = new Date();
            trintaDiasAtras.setDate(trintaDiasAtras.getDate() - 30);
            const novosCadastros = pessoas.filter(p => new Date(p.DataCadastro) >= trintaDiasAtras).length;
            // Distribuição por sexo
            let masculino = 0, feminino = 0, naoInformado = 0;
            // Idade média
            let somaIdades = 0, totalComIdade = 0;
            pessoas.forEach(p => {
                if (p.Sexo === 1) masculino++;
                else if (p.Sexo === 2) feminino++;
                else naoInformado++;
                
                // Calcula idade a partir da data de nascimento
                const idade = calcularIdade(p.DataNascimento);
                if (idade !== null && !isNaN(idade)) {
                    somaIdades += idade;
                    totalComIdade++;
                }
            });
            const idadeMedia = totalComIdade > 0 ? Math.round(somaIdades / totalComIdade) : 0;
            setStats({
                totalPessoas,
                novosCadastros,
                idadeMedia,
                sexo: { masculino, feminino, naoInformado },
            });
        } catch (e) {
            // fallback: mantém stats antigos
        } finally {
            setLoading(false);
        }
    }, []);

    React.useEffect(() => {
        fetchStats();
    }, [fetchStats]);

    useDashboardAutoRefresh(fetchStats, 10000);

    // Dados do gráfico de sexo
    const pieData = {
        labels: ['Feminino', 'Masculino', 'Não informado'],
        datasets: [
            {
                data: [
                    stats.sexo.feminino,
                    stats.sexo.masculino,
                    stats.sexo.naoInformado,
                ],
                backgroundColor: ['#a855f7', '#3b82f6', '#a3a3a3'],
                borderWidth: 1,
            },
        ],
    };

    // Cards
    const cards = [
        {
            label: 'Total de Pessoas',
            value: stats.totalPessoas,
            bg: 'indigo.100',
            color: 'indigo.500',
            icon: FaUsers,
            change: '',
            changeColor: 'gray.500',
            since: 'Cadastradas no sistema',
        },
        {
            label: 'Novos Cadastros',
            value: stats.novosCadastros,
            bg: 'blue.100',
            color: 'blue.500',
            icon: FaUserPlus,
            change: '',
            changeColor: 'gray.500',
            since: 'Últimos 30 dias',
        },
        {
            label: 'Idade Média',
            value: stats.idadeMedia,
            bg: 'teal.100',
            color: 'teal.500',
            icon: FaCheckCircle,
            change: '',
            changeColor: 'gray.500',
            since: 'anos',
        },
    ];

    return (
        <Flex>
            <SideBar isOpen={isOpen} onClose={onClose} />
            <Flex direction="column" flex={1}>
                <Header onOpenSidebar={onOpen} />
                <Box as="main" p={6} bg={bgMain}>
                    {/* Cards de resumo */}
                    <Grid
                        templateColumns={{
                            base: '1fr',
                            md: 'repeat(2, 1fr)',
                            lg: 'repeat(3, 1fr)',
                        }}
                        gap={6}
                        mb={6}
                    >
                        {cards.map((card) => (
                            <Box
                                key={card.label}
                                bg="white"
                                rounded="lg"
                                shadow="md"
                                p={6}
                                display="flex"
                                flexDirection="column"
                                alignItems={'flex-start'}
                            >
                                <Flex align="center">
                                    <Box
                                        p={3}
                                        rounded="full"
                                        bg={card.bg}
                                        color={card.color}
                                        mr={4}
                                    >
                                        <card.icon size={24} />
                                    </Box>
                                    <Box>
                                        <Text color="gray.500">{card.label}</Text>
                                        <Text fontSize="2xl" fontWeight="bold">
                                            {card.value}
                                        </Text>
                                    </Box>
                                </Flex>
                                <HStack mt={4} spacing={2}>
                                    <Text
                                        color={card.changeColor}
                                        fontSize="sm"
                                        fontWeight="semibold"
                                    >
                                        {card.change}
                                    </Text>
                                    <Text color="gray.500" fontSize="sm">
                                        {card.since}
                                    </Text>
                                </HStack>
                            </Box>
                        ))}
                    </Grid>

                    {/* Gráfico de pizza em linha separada */}
                    <Box
                        bg="white"
                        rounded="lg"
                        shadow="md"
                        p={6}
                        mb={6}
                        display="flex"
                        flexDirection="column"
                        alignItems="center"
                        maxW="400px"
                        mx="auto"
                    >
                        <Flex align="center" mb={4}>
                            <Box
                                p={3}
                                rounded="full"
                                bg="purple.100"
                                color="purple.500"
                                mr={4}
                            >
                                <FaChartPie size={24} />
                            </Box>
                            <Text color="gray.500" fontWeight="bold" fontSize="lg">
                                Distribuição por Sexo
                            </Text>
                        </Flex>
                        <Box w="100%" maxW="250px">
                            <Pie
                                data={pieData}
                                options={{
                                    plugins: { legend: { position: 'bottom' } },
                                }}
                            />
                        </Box>
                    </Box>
                </Box>
            </Flex>
        </Flex>
    );
}
