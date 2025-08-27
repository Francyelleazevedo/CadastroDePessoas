import React, { createContext, useState, useCallback } from 'react';
import { useToast } from '@chakra-ui/react';

export const NotificationContext = createContext();

export const NotificationProvider = ({ children }) => {
    const toast = useToast();
    const [notifications, setNotifications] = useState([]);

    const showSuccess = useCallback((message, title = 'Sucesso') => {
        const id = `success-${Date.now()}-${Math.random()}`;
        toast({
            id,
            title,
            description: message,
            status: 'success',
            duration: 3000,
            isClosable: true,
            position: 'top-right',
        });
    }, [toast]);

    const showError = useCallback((message, title = 'Erro') => {
        const id = `error-${Date.now()}-${Math.random()}`;
        toast({
            id,
            title,
            description: message,
            status: 'error',
            duration: 5000,
            isClosable: true,
            position: 'top-right',
        });
    }, [toast]);

    const showWarning = useCallback((message, title = 'Atenção') => {
        const id = `warning-${Date.now()}-${Math.random()}`;
        toast({
            id,
            title,
            description: message,
            status: 'warning',
            duration: 4000,
            isClosable: true,
            position: 'top-right',
        });
    }, [toast]);

    const showInfo = useCallback((message, title = 'Informação') => {
        const id = `info-${Date.now()}-${Math.random()}`;
        toast({
            id,
            title,
            description: message,
            status: 'info',
            duration: 3000,
            isClosable: true,
            position: 'top-right',
        });
    }, [toast]);

    return (
        <NotificationContext.Provider value={{
            notifications,
            showSuccess,
            showError,
            showWarning,
            showInfo
        }}>
            {children}
        </NotificationContext.Provider>
    );
};