import http from 'k6/http';

import auth from './auth.js';
import { baseUrl, params, options as commonOptions } from './config.js';

export const options = Object.assign(commonOptions, {
  duration: '30s',
  vus: 5
});

export default function () {
  auth();

  const res = http.post(
    `${baseUrl}/api/Attendance/passive`,
    JSON.stringify({
      stationId: 1,
      serialNumber: 1
    }),
    params
  );
}
