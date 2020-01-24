import { Injectable, Directive } from '@angular/core';

@Injectable()
export class LanguageService {
    private lang: string= 'vn';
  
    constructor() { }

    setLanguage(val) {
        this.lang = val;
    }

    getLanguage() {
        return this.lang;
    }
  }