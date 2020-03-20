import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FollowingServiceComponent } from './following-service.component';

describe('FollowingServiceComponent', () => {
  let component: FollowingServiceComponent;
  let fixture: ComponentFixture<FollowingServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FollowingServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FollowingServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
