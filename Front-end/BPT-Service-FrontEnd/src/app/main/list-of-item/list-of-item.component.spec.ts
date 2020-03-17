import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListOfItemComponent } from './list-of-item.component';

describe('ListOfItemComponent', () => {
  let component: ListOfItemComponent;
  let fixture: ComponentFixture<ListOfItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListOfItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListOfItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
