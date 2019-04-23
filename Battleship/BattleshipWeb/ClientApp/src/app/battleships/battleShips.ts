import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'home',
  templateUrl: './battleships.html'
})

export class BattleshipsComponent {
  public tiles: number[];

  constructor() {
    this.tiles = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
  }
}

