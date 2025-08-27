import React from 'react';
import {
    Box,
    Flex,
    IconButton,
    Text,
    HStack,
    Avatar,
    Badge,
    useColorModeValue,
    Menu,
    MenuButton,
    MenuList,
    MenuItem,
    MenuDivider,
} from '@chakra-ui/react';
import { FaBars, FaBell, FaUser, FaSignOutAlt } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';

const Header = ({ onOpenSidebar }) => {
    const bg = useColorModeValue('white', 'gray.800');
    const shadow = useColorModeValue('sm', 'sm-dark');
    const navigate = useNavigate();

    let user = null;
    try {
        user = JSON.parse(localStorage.getItem('user'));
    } catch { }
    const nome = user?.nome || user?.name || 'Administrador';
    const email = user?.email || 'admin@exemplo.com';

    const handleLogout = async () => {
        await authService.logout();
        navigate('/login');
    };

    return (
        <Box as="header" bg={bg} boxShadow={shadow} w="full">
            <Flex align="center" justify="space-between" px={6} py={4}>
                <HStack spacing={4}>
                    <IconButton
                        display={{ base: 'inline-flex', md: 'none' }}
                        aria-label="Abrir menu"
                        icon={<FaBars />}
                        variant="ghost"
                        fontSize="xl"
                        onClick={onOpenSidebar}
                    />
                    <Text fontSize="xl" fontWeight="semibold" color="gray.800">
                        Dashboard
                    </Text>
                </HStack>
                <HStack spacing={4}>
                    <Menu>
                        <MenuButton as={Box} p={0} borderRadius="full" _focus={{ boxShadow: 'none' }} cursor="pointer">
                            <Avatar size="md" bgGradient="linear(to-br, blue.400, purple.500)" icon={<FaUser fontSize="1.2rem" color="white" />} />
                        </MenuButton>
                        <MenuList minW="220px" p={0}>
                            <Box px={4} py={3}>
                                <Text fontWeight="bold" fontSize="md" noOfLines={1}>{nome}</Text>
                                <Text fontSize="sm" color="gray.500" noOfLines={1}>{email}</Text>
                            </Box>
                            <MenuDivider />
                            <MenuItem icon={<FaUser />} onClick={() => navigate('/meu-perfil')}>
                                Meu Perfil
                            </MenuItem>
                            <MenuItem icon={<FaSignOutAlt />} color="red.500" onClick={handleLogout}>
                                Sair da Conta
                            </MenuItem>
                        </MenuList>
                    </Menu>
                </HStack>
            </Flex>
        </Box>
    );
};

export default Header;
