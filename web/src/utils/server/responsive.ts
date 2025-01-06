import { UAParser } from 'ua-parser-js';

/**
 * check mobile device in server
 */
export const isMobileDevice = async () => {
    const device = new UAParser('').getDevice();

    return device.type === 'mobile';
};

/**
 * get device info in server
 */
export const gerServerDeviceInfo = async () => {
    const parser = new UAParser('');

    return {
        browser: parser.getBrowser().name,
        isMobile: isMobileDevice(),
        os: parser.getOS().name,
    };
};
