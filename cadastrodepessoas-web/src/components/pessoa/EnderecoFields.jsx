import React from 'react';
import {
    Box,
    FormControl,
    FormLabel,
    Input,
    SimpleGrid,
    FormErrorMessage,
} from '@chakra-ui/react';
import {
    FaMapMarkerAlt,
    FaRoad,
    FaBuilding,
    FaCity,
    FaMapPin,
} from 'react-icons/fa';
import { maskCEP } from '../../utils';

export const EnderecoFields = ({ register, errors, setValue, isRequired = false }) => {
    const requiredMark = isRequired ? ' *' : '';
    
    const handleCepChange = (e) => {
        const maskedValue = maskCEP(e.target.value);
        setValue('endereco.cep', maskedValue);
    };
    
    return (
        <>
            <FormControl isInvalid={!!errors?.logradouro}>
                <FormLabel display="flex" alignItems="center">
                    <Box as={FaRoad} mr={2} color="gray.500" />
                    Logradouro{requiredMark}
                </FormLabel>
                <Input
                    {...register('endereco.logradouro')}
                    placeholder="Ex: Rua das Flores"
                    size="lg"
                />
                <FormErrorMessage>
                    {errors?.logradouro?.message}
                </FormErrorMessage>
            </FormControl>

            <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
                <FormControl isInvalid={!!errors?.numero}>
                    <FormLabel display="flex" alignItems="center">
                        <Box as={FaBuilding} mr={2} color="gray.500" />
                        Número{requiredMark}
                    </FormLabel>
                    <Input
                        {...register('endereco.numero')}
                        placeholder="123"
                        size="lg"
                    />
                    <FormErrorMessage>
                        {errors?.numero?.message}
                    </FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors?.complemento}>
                    <FormLabel>Complemento</FormLabel>
                    <Input
                        {...register('endereco.complemento')}
                        placeholder="Apto 101"
                        size="lg"
                    />
                    <FormErrorMessage>
                        {errors?.complemento?.message}
                    </FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors?.cep}>
                    <FormLabel display="flex" alignItems="center">
                        <Box as={FaMapPin} mr={2} color="gray.500" />
                        CEP{requiredMark}
                    </FormLabel>
                    <Input
                        {...register('endereco.cep')}
                        placeholder="00000-000"
                        size="lg"
                        maxLength={9}
                        onChange={handleCepChange}
                    />
                    <FormErrorMessage>
                        {errors?.cep?.message}
                    </FormErrorMessage>
                </FormControl>
            </SimpleGrid>

            <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
                <FormControl isInvalid={!!errors?.bairro}>
                    <FormLabel display="flex" alignItems="center">
                        <Box as={FaMapMarkerAlt} mr={2} color="gray.500" />
                        Bairro{requiredMark}
                    </FormLabel>
                    <Input
                        {...register('endereco.bairro')}
                        placeholder="Centro"
                        size="lg"
                    />
                    <FormErrorMessage>
                        {errors?.bairro?.message}
                    </FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors?.cidade}>
                    <FormLabel display="flex" alignItems="center">
                        <Box as={FaCity} mr={2} color="gray.500" />
                        Cidade{requiredMark}
                    </FormLabel>
                    <Input
                        {...register('endereco.cidade')}
                        placeholder="São Paulo"
                        size="lg"
                    />
                    <FormErrorMessage>
                        {errors?.cidade?.message}
                    </FormErrorMessage>
                </FormControl>

                <FormControl isInvalid={!!errors?.estado}>
                    <FormLabel>Estado (UF){requiredMark}</FormLabel>
                    <Input
                        {...register('endereco.estado')}
                        placeholder="PE"
                        size="lg"
                        maxLength={2}
                        style={{ textTransform: 'uppercase' }}
                    />
                    <FormErrorMessage>
                        {errors?.estado?.message}
                    </FormErrorMessage>
                </FormControl>
            </SimpleGrid>
        </>
    );
};
