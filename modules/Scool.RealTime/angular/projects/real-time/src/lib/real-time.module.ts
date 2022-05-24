import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { RealTimeComponent } from './components/real-time.component';
import { RealTimeRoutingModule } from './real-time-routing.module';

@NgModule({
  declarations: [RealTimeComponent],
  imports: [CoreModule, ThemeSharedModule, RealTimeRoutingModule],
  exports: [RealTimeComponent],
})
export class RealTimeModule {
  static forChild(): ModuleWithProviders<RealTimeModule> {
    return {
      ngModule: RealTimeModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<RealTimeModule> {
    return new LazyModuleFactory(RealTimeModule.forChild());
  }
}
