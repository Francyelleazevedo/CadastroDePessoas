import { useEffect, useRef } from 'react';

/**
 * Hook para atualizar dados do dashboard em intervalos regulares.
 * @param {Function} callback Função que será chamada a cada intervalo.
 * @param {number} delay Delay em milissegundos (ex: 10000 para 10s)
 */
export function useDashboardAutoRefresh(callback, delay = 10000) {
  const savedCallback = useRef();

  useEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  useEffect(() => {
    if (delay === null) return;
    const tick = () => savedCallback.current();
    const id = setInterval(tick, delay);
    return () => clearInterval(id);
  }, [delay]);
}
