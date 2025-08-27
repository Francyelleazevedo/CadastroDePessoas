import React from 'react';
import {
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalFooter,
    ModalBody,
    ModalCloseButton,
    Button,
    Text,
    VStack,
    HStack,
    Box,
    useColorModeValue,
} from '@chakra-ui/react';
import {
    FaTrash,
    FaExclamationTriangle,
} from 'react-icons/fa';

export default function ConfirmarExclusaoModal({
    isOpen,
    onClose,
    onConfirm,
    pessoa,
    isLoading = false,
}) {
    const handleConfirm = () => {
        if (pessoa) {
            onConfirm(pessoa.Id, pessoa.Nome);
        }
    };
    
    if (!pessoa) {
        return null;
    }

    return (
        <Modal isOpen={isOpen} onClose={onClose} size="sm" isCentered>
            <ModalOverlay bg="blackAlpha.600" />
            <ModalContent>
                <ModalHeader>
                    <HStack spacing={3}>
                        <Box color="red.500">
                            <FaExclamationTriangle size={20} />
                        </Box>
                        <Text fontSize="lg" fontWeight="bold">
                            Confirmar Exclusão
                        </Text>
                    </HStack>
                </ModalHeader>
                
                <ModalCloseButton />
                
                <ModalBody>
                    <VStack spacing={4} align="stretch">
                        <Text color="gray.600" textAlign="center">
                            Tem certeza que deseja excluir esta pessoa?
                        </Text>
                    </VStack>
                </ModalBody>

                <ModalFooter>
                    <HStack spacing={3}>
                        <Button
                            variant="outline"
                            onClick={onClose}
                            disabled={isLoading}
                        >
                            Cancelar
                        </Button>
                        <Button
                            colorScheme="red"
                            leftIcon={<FaTrash />}
                            onClick={handleConfirm}
                            isLoading={isLoading}
                            loadingText="Excluindo..."
                        >
                            Excluir
                        </Button>
                    </HStack>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
}
