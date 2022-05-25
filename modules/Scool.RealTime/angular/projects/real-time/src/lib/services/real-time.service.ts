import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class RealTimeService {
  apiName = 'RealTime';

  constructor(private restService: RestService) {}

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/RealTime/sample' },
      { apiName: this.apiName }
    );
  }
}