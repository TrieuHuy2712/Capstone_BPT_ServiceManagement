import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopbarUserComponent } from './topbar-user.component';

describe('TopbarUserComponent', () => {
  let component: TopbarUserComponent;
  let fixture: ComponentFixture<TopbarUserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopbarUserComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopbarUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
