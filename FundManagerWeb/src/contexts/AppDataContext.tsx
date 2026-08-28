import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import { staffDeviceService } from '../services/deviceMappingService';
import { signalRService } from '../services/signalRService';
import type { CurrentStaffDeviceResponse } from '../types/deviceType';
import type { OutletResponse } from '../types/outletType';
import { outletService } from '../services/outletService';

interface AppDataContextType {
    staffDevice: CurrentStaffDeviceResponse | null;
    outlets: OutletResponse[];
    loading: boolean;
    error: string | null;
    refetchStaffDevice: () => Promise<void>;
    refetchOutlets: () => Promise<void>;
}

const AppDataContext = createContext<AppDataContextType | undefined>(undefined);

export const AppDataProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [staffDevice, setStaffDevice] = useState<CurrentStaffDeviceResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [outlets, setOutlets] = useState<OutletResponse[]>([]);

    const fetchOutlets = async () => {
        try {
            const data = await outletService.getListAsync();
            setOutlets(data);
        } catch (err) {
            console.error('Error fetching outlets:', err);
            setError('Failed to load outlets');
        }
    };

    const fetchStaffDevice = async () => {
        try {
            // console.log('🔄 [AppDataContext] Fetching staff device...');
            const data = await staffDeviceService.getCurrentStaffDeviceAsync();
            // console.log('✅ [AppDataContext] Staff device loaded:', data);
            // console.log('📱 [AppDataContext] staffDeviceId:', data?.staffDeviceId);
            // console.log('💻 [AppDataContext] deviceName:', data?.deviceName);
            setStaffDevice(data);

            // ✅ Initialize SignalR IMMEDIATELY after getting staffDevice from API
            if (data?.staffDeviceId && data?.deviceName) {
                // console.log('🚀 [AppDataContext] Initializing SignalR with API data...');
                // console.log('   staffDeviceId:', data.staffDeviceId);
                // console.log('   deviceName:', data.deviceName);

                try {
                    await signalRService.startConnection(data.staffDeviceId, data.deviceName);
                    // console.log('✅ [AppDataContext] SignalR initialized successfully');
                } catch (signalRError) {
                    console.error('❌ [AppDataContext] SignalR initialization failed:', signalRError);
                    // Don't throw - SignalR failure shouldn't block app loading
                }
            } else {
                console.warn('⚠️ [AppDataContext] No staffDeviceId or deviceName - skipping SignalR initialization');
            }
        } catch (err) {
            console.error('❌ [AppDataContext] Error fetching staff device:', err);
            setError('Failed to load staff device');
        }
    };

    useEffect(() => {
        const loadInitialData = async () => {
            console.log('🚀 [AppDataContext] Loading initial data...');
            setLoading(true);
            setError(null);

            await Promise.all([
                fetchStaffDevice(), // This now also initializes SignalR
                fetchOutlets() // Fetch outlets for mapping
            ]);

            setLoading(false);
            console.log('✅ [AppDataContext] Initial data loaded successfully');
        };

        loadInitialData();
    }, []);

    const value = {
        staffDevice,
        outlets,
        loading,
        error,
        refetchStaffDevice: fetchStaffDevice,
        refetchOutlets: fetchOutlets
    };

    return (
        <AppDataContext.Provider value={value}>
            {children}
        </AppDataContext.Provider>
    );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAppData = () => {
    const context = useContext(AppDataContext);
    if (context === undefined) {
        throw new Error('useAppData must be used within an AppDataProvider');
    }
    return context;
};