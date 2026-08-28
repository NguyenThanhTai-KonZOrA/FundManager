type RuntimeEnv = {
	API_BASE?: string;
	ONLINE_API_BASE?: string;
	USE_MOCK_DATA?: boolean | string;
};

declare global {
	interface Window {
		_env_?: RuntimeEnv;
	}
}

const getRuntimeEnv = (): RuntimeEnv => window._env_ ?? {};

export const getApiBase = (): string => getRuntimeEnv().API_BASE ?? '';

export const getOnlineApiBase = (): string => getRuntimeEnv().ONLINE_API_BASE ?? '';

export const getUseMockData = (): boolean => {
	const value = getRuntimeEnv().USE_MOCK_DATA;
	if (typeof value === 'boolean') {
		return value;
	}

	if (typeof value === 'string') {
		return value.toLowerCase() === 'true';
	}

	return false;
};
