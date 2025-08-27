
import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../../services/authService';
import {
    Box,
    Flex,
    VStack,
    HStack,
    Text,
    IconButton,
    Avatar,
    useColorModeValue,
    Collapse,
    Badge,
    Button,
    Divider,
    Link,
    useBreakpointValue,
    Drawer,
    DrawerBody,
    DrawerHeader,
    DrawerOverlay,
    DrawerContent,
    DrawerCloseButton,
    useDisclosure,
} from '@chakra-ui/react';
import {
    FaTachometerAlt,
    FaUsers,
    FaChartLine,
    FaCog,
    FaEnvelope,
    FaSignOutAlt,
    FaUser,
    FaUserEdit,
    FaBars,
    FaTimes,
} from 'react-icons/fa';

const menuItems = [
    {
        label: 'Dashboard',
        icon: FaTachometerAlt,
        href: '/',
    },
    {
        label: 'Pessoas',
        icon: FaUsers,
        children: [
            {
                label: 'Listagem',
                icon: FaChartLine,
                href: '/pessoas',
            },
            {
                label: 'Criação',
                icon: FaUser,
                href: '/pessoas/criar',
            },
        ],
    },
    {
        label: 'Meu Perfil',
        icon: FaUserEdit,
        href: '/meu-perfil',
    },
];

const SideBar = ({ isOpen, onClose }) => {
    const bg = useColorModeValue('white', 'gray.800');
    const activeBg = useColorModeValue('indigo.100', 'gray.700');
    const activeColor = useColorModeValue('indigo.500', 'indigo.200');
    const hoverBg = useColorModeValue('indigo.100', 'gray.700');
    const hoverColor = useColorModeValue('indigo.500', 'indigo.200');
    const navigate = useNavigate();
    const location = useLocation();
    
    const [isCollapsed, setIsCollapsed] = React.useState(false);
    
    const [openMenu, setOpenMenu] = React.useState(null);

    const isMobile = useBreakpointValue({ base: true, md: false });

    const handleLogout = async (e) => {
        e.preventDefault();
        await authService.logout();
        navigate('/login');
    };

    let user = null;
    try {
        user = JSON.parse(localStorage.getItem('user'));
    } catch { }

    const nome = user?.nome || user?.name || 'Usuário';
    const email = user?.email || 'sem-email';

    const isItemActive = (href, hasChildren) => {
        if (hasChildren) return false;
        return location.pathname === href;
    };

    const handleMenuClick = (idx, hasChildren, href) => {
        if (hasChildren) {
            setOpenMenu(openMenu === idx ? null : idx);
        } else if (href && href !== '#') {
            navigate(href);

            if (isMobile && onClose) {
                onClose();
            }
        }
    };

    const handleChildClick = (href) => {
        if (href && href !== '#') {
            navigate(href);

            if (isMobile && onClose) {
                onClose();
            }
        }
    };

    const toggleCollapse = () => {
        setIsCollapsed(!isCollapsed);
    };

    const sidebarContent = (
        <>
            <Flex 
                align="center" 
                justify={isCollapsed ? "center" : "space-between"} 
                h={16} 
                bgGradient="linear(to-br, blue.400, purple.500)"
                px={isCollapsed ? 2 : 4}
            >
                {!isCollapsed && (
                    <Text color="white" fontWeight="bold" fontSize="xl">
                        Dashboard
                    </Text>
                )}
                {!isMobile && (
                    <IconButton
                        aria-label="Toggle sidebar"
                        icon={isCollapsed ? <FaBars /> : <FaTimes />}
                        variant="ghost"
                        color="white"
                        size="sm"
                        onClick={toggleCollapse}
                        _hover={{ bg: 'whiteAlpha.200' }}
                    />
                )}
            </Flex>

            <VStack align="stretch" spacing={4} p={isCollapsed ? 2 : 4}>
                <VStack align="stretch" spacing={1}>
                    {menuItems.map((item, idx) => {
                        const isActive = isItemActive(item.href, !!item.children);
                        return (
                            <Box key={item.label}>
                                <Box
                                    as="button"
                                    w="100%"
                                    textAlign="left"
                                    _hover={{ bg: hoverBg, color: hoverColor, textDecoration: 'none' }}
                                    bg={isActive ? activeBg : 'transparent'}
                                    color={isActive ? activeColor : 'inherit'}
                                    fontWeight={isActive ? 'bold' : 'normal'}
                                    p={3}
                                    borderRadius="lg"
                                    display="flex"
                                    alignItems="center"
                                    mb={2}
                                    position="relative"
                                    onClick={() => handleMenuClick(idx, !!item.children, item.href)}
                                    title={isCollapsed ? item.label : undefined}
                                >
                                    <Box as={item.icon} mr={isCollapsed ? 0 : 3} fontSize="lg" />
                                    {!isCollapsed && (
                                        <>
                                            <Text flex={1}>{item.label}</Text>
                                            {item.children && (
                                                <Box ml={2} fontSize="sm" color="gray.400">
                                                    {openMenu === idx ? '▲' : '▼'}
                                                </Box>
                                            )}
                                            {item.badge && (
                                                <Badge ml="auto" colorScheme="blue" fontSize="0.7em">
                                                    {item.badge}
                                                </Badge>
                                            )}
                                        </>
                                    )}
                                </Box>
                                {item.children && !isCollapsed && (
                                    <Collapse in={openMenu === idx} animateOpacity>
                                        <VStack align="stretch" pl={8} spacing={1} mb={2}>
                                            {item.children.map((child) => (
                                                <Box
                                                    key={child.label}
                                                    as="button"
                                                    w="100%"
                                                    textAlign="left"
                                                    _hover={{ bg: hoverBg, color: hoverColor, textDecoration: 'none' }}
                                                    p={2}
                                                    borderRadius="md"
                                                    display="flex"
                                                    alignItems="center"
                                                    onClick={() => handleChildClick(child.href)}
                                                >
                                                    <Box as={child.icon} mr={2} fontSize="md" />
                                                    <Text>{child.label}</Text>
                                                </Box>
                                            ))}
                                        </VStack>
                                    </Collapse>
                                )}
                            </Box>
                        );
                    })}
                </VStack>

                <Divider />
                <Button
                    leftIcon={<FaSignOutAlt />}
                    variant="ghost"
                    colorScheme="red"
                    size="sm"
                    onClick={handleLogout}
                    justifyContent={isCollapsed ? "center" : "flex-start"}
                    title={isCollapsed ? "Sair" : undefined}
                >
                    {!isCollapsed && "Sair"}
                </Button>
            </VStack>
        </>
    );

    if (isMobile) {
        return (
            <Drawer isOpen={isOpen} placement="left" onClose={onClose}>
                <DrawerOverlay />
                <DrawerContent maxW="64">
                    <DrawerCloseButton />
                    <Box bg={bg} h="100%">
                        {sidebarContent}
                    </Box>
                </DrawerContent>
            </Drawer>
        );
    }

    return (
        <Box
            as="nav"
            w={isCollapsed ? 16 : 64}
            minH="100vh"
            bg={bg}
            boxShadow="lg"
            transition="all 0.3s"
            position="relative"
        >
            {sidebarContent}
        </Box>
    );
};

export default SideBar;
