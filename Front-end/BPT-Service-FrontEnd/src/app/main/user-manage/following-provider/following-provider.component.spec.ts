import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FollowingProviderComponent } from './following-provider.component';

describe('FollowingProviderComponent', () => {
  let component: FollowingProviderComponent;
  let fixture: ComponentFixture<FollowingProviderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FollowingProviderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FollowingProviderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
