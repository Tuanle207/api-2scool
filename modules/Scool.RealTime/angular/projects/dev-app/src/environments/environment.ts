import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl: 'http://localhost:4200/',
    name: 'RealTime',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44352',
    redirectUri: baseUrl,
    clientId: 'RealTime_App',
    responseType: 'code',
    scope: 'offline_access RealTime role email openid profile',
  },
  apis: {
    default: {
      url: 'https://localhost:44352',
      rootNamespace: 'Scool.RealTime',
    },
    RealTime: {
      url: 'https://localhost:44397',
      rootNamespace: 'Scool.RealTime',
    },
  },
} as Environment;
