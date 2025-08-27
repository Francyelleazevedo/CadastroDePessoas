import React from 'react';
import { Box, Button, Flex, Heading, Text, useColorModeValue } from '@chakra-ui/react';
import { FaExclamationTriangle } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';

const PaginaNaoEncontrada = () => {
	const navigate = useNavigate();
	const bg = useColorModeValue('gray.100', 'gray.900');
	const cardBg = useColorModeValue('white', 'gray.800');
	const iconColor = useColorModeValue('purple.500', 'purple.300');

	return (
		<Flex minH="100vh" align="center" justify="center" bg={bg} p={4}>
			<Box
				bg={cardBg}
				rounded="2xl"
				shadow="2xl"
				w="full"
				maxW="md"
				p={10}
				textAlign="center"
			>
				<Box mb={6} display="flex" alignItems="center" justifyContent="center">
					<Box bg="purple.100" rounded="full" p={4} display="inline-flex">
						<FaExclamationTriangle size={48} color={iconColor} />
					</Box>
				</Box>
				<Heading as="h1" size="2xl" color="purple.600" mb={2}>
					404
				</Heading>
				<Text fontSize="xl" fontWeight="bold" color="gray.700" mb={2}>
					Página não encontrada
				</Text>
				<Text color="gray.500" mb={6}>
					O endereço que você tentou acessar não existe ou foi removido.
				</Text>
				<Button
					colorScheme="purple"
					size="lg"
					onClick={() => navigate('/')}
				>
					Voltar para o início
				</Button>
			</Box>
		</Flex>
	);
};

export default PaginaNaoEncontrada;
