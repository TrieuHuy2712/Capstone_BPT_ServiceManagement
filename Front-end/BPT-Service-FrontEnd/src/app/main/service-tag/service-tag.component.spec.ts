import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceTagComponent } from './service-tag.component';

describe('ServiceTagComponent', () => {
  let component: ServiceTagComponent;
  let fixture: ComponentFixture<ServiceTagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceTagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
