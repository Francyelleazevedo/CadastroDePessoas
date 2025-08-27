import React, { useState } from 'react';
import { Box, Flex, useDisclosure, useBreakpointValue } from '@chakra-ui/react';
import { Outlet } from 'react-router-dom';
import Header from './Header';
import SideBar from './SideBar';
import Footer from './Footer';

const Layout = () => {
    const { isOpen, onToggle } = useDisclosure();
    const sidebarWidth = useBreakpointValue({ base: 'full', md: '280px' });

    return (
        <Flex direction="column" minH="100vh" bg="gray.50">
            <Header onSidebarToggle={onToggle} />

            <Flex flex="1" mt="72px" position="relative">
                <SideBar isOpen={isOpen} />

                <Box
                    flex="1"
                    ml={{ base: 0, md: sidebarWidth }}
                    transition="margin-left 0.3s cubic-bezier(.08,.52,.52,1)"
                    minH="calc(100vh - 72px)"
                >
                    {/* Overlay para mobile quando sidebar está aberta */}
                    {isOpen && (
                        <Box
                            position="fixed"
                            top="72px"
                            left="0"
                            right="0"
                            bottom="0"
                            bg="blackAlpha.600"
                            zIndex={800}
                            display={{ base: 'block', md: 'none' }}
                            onClick={onToggle}
                        />
                    )}

                    {/* Conteúdo principal com padding adequado */}
                    <Box
                        minH="calc(100vh - 72px)"
                        bg="white"
                        m={{ base: 4, md: 6 }}
                        borderRadius="2xl"
                        boxShadow="base"
                        border="1px solid"
                        borderColor="gray.100"
                        overflow="hidden"
                        p={{ base: 4, md: 6 }}
                    >
                        <Outlet />
                    </Box>
                </Box>
            </Flex>

            <Footer />
        </Flex>
    );
};

export default Layout;