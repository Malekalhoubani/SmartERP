const gatewayApiUrl = 'https://localhost:7290/api';

export const environment = {
  gatewayApiUrl,
  identityApiUrl: `${gatewayApiUrl}/identity/api/auth`,
  organizationApiUrl: `${gatewayApiUrl}/organization/api`,
  inventoryApiUrl: `${gatewayApiUrl}/inventory/api`,
};
