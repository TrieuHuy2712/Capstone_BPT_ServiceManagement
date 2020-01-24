import { Injectable } from "@angular/core";
import { LanguageService } from './language.service';

@Injectable()
export class TranslationSet {
  public languange: string;
  public values: { [key: string]: string } = {};
}
export class TranslationService {
  public languages = ["vn", "en"];

  public language = "vn";

  private dictionary: { [key: string]: TranslationSet } = {
    vn: {
      languange: "vn",
      values: {
        add: "Thêm"
      }
    },
    eng: {
      languange: "eng",
      values: {
        add: "Add"
      }
    }
  };

  constructor(private languageService: LanguageService) {}

  translate(key: string): string {

    if (this.languageService.getLanguage() == "vn") {
      return this.dictionary[this.languageService.getLanguage()].values[key];
    }else{
        return this.dictionary["en"].values[key];
    }
  }
}
