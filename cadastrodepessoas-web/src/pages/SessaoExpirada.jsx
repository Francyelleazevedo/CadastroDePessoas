import React from 'react';
import { Box, Button, Flex, Heading, Text, useColorModeValue } from '@chakra-ui/react';
import { FaClock } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';

const SessaoExpirada = () => {
	const navigate = useNavigate();
	const bg = useColorModeValue('gray.100', 'gray.900');
	const cardBg = useColorModeValue('white', 'gray.800');
	const iconColor = useColorModeValue('blue.500', 'blue.300');

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
					<Box bg="blue.100" rounded="full" p={4} display="inline-flex">
						<FaClock size={48} color={iconColor} />
					</Box>
				</Box>
				<Heading as="h1" size="xl" color="blue.600" mb={2}>
					Sessão expirada
				</Heading>
				<Text fontSize="lg" fontWeight="bold" color="gray.700" mb={2}>
					Sua sessão foi encerrada por inatividade ou expiração do login.
				</Text>
				<Text color="gray.500" mb={6}>
					Por segurança, você precisa entrar novamente para continuar usando o sistema.
				</Text>
				<Button
					colorScheme="blue"
					size="lg"
					onClick={() => navigate('/login')}
				>
					Fazer login novamente
				</Button>
			</Box>
		</Flex>
	);
};

export default SessaoExpirada;
