import React from 'react';
import { Box, Text, useColorModeValue, Flex } from '@chakra-ui/react';
import { useLocation } from 'react-router-dom';

const Footer = () => {
	const bg = useColorModeValue('white', 'gray.800');
	const color = useColorModeValue('gray.600', 'gray.400');
	const location = useLocation();
	
	const routesWithoutFooter = [
		'/login',
		'/registrar',
		'/sessao-expirada',
		'/erro-servidor',
		'/erro-conexao'
	];
	
	if (routesWithoutFooter.includes(location.pathname)) {
		return null;
	}

	return (
		<Box 
			as="footer" 
			bg={bg} 
			w="full"
			h="60px"
			py={4} 
			boxShadow="sm"
			mt="auto"
		>
			<Flex align="center" justify="center" h="full">
				<Text fontSize="sm" color={color}>
					&copy; {new Date().getFullYear()} Desenvolvido por Francyelle Azevedo. Todos os direitos reservados.
				</Text>
			</Flex>
		</Box>
	);
};

export default Footer;
