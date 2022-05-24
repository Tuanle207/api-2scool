import { ModuleWithProviders, NgModule } from '@angular/core';
import { REAL_TIME_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class RealTimeConfigModule {
  static forRoot(): ModuleWithProviders<RealTimeConfigModule> {
    return {
      ngModule: RealTimeConfigModule,
      providers: [REAL_TIME_ROUTE_PROVIDERS],
    };
  }
}
