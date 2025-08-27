import React, { useEffect } from 'react';
import { BrowserRouter as Router, useNavigate } from 'react-router-dom';
import { ChakraProvider, Flex, Box } from '@chakra-ui/react';
import { AuthProvider } from './contexts/AuthContext.jsx';
import { NotificationProvider } from './contexts/NotificationContext';
import { useNotification } from './hooks/useNotification';
import { setupApiErrorHandling } from './services/api';
import AppRoutes from './routes.jsx';
import Footer from './components/layout/Footer';

const ApiErrorHandler = () => {
    const navigate = useNavigate();
    const notificationService = useNotification();
    
    useEffect(() => {
        setupApiErrorHandling(notificationService, navigate);
    }, [navigate, notificationService]);
    
    return null;
};

const App = () => {
    return (
        <ChakraProvider>
            <Router>
                <Flex direction="column" minH="100vh">
                    <Box flex="1">
                        <AuthProvider>
                            <NotificationProvider>
                                <ApiErrorHandler />
                                <AppRoutes />
                            </NotificationProvider>
                        </AuthProvider>
                    </Box>
                    <Footer />
                </Flex>
            </Router>
        </ChakraProvider>
    );
};

export default App;