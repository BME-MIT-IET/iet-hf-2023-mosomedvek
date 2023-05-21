import http from 'k6/http';

import { baseUrl, email, password, params } from './config.js';

export default function auth() {
  return http.post(
    `${baseUrl}/api/User/Login`,
    JSON.stringify({
      email: email,
      password: password
    }),
    params
  );
}
