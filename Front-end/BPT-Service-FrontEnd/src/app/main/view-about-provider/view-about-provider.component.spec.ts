import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAboutProviderComponent } from './view-about-provider.component';

describe('ViewAboutProviderComponent', () => {
  let component: ViewAboutProviderComponent;
  let fixture: ComponentFixture<ViewAboutProviderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewAboutProviderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAboutProviderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
