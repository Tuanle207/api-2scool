import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RealTimeComponent } from './real-time.component';

describe('RealTimeComponent', () => {
  let component: RealTimeComponent;
  let fixture: ComponentFixture<RealTimeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RealTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RealTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
