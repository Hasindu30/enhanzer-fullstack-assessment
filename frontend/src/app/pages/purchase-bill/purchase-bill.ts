import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="placeholder-page">
      <h2>Purchase Bill</h2>
      <p>You are logged in. Purchase Bill form will be implemented here.</p>
    </div>
  `,
  styles: [`
    .placeholder-page {
      padding: 2rem;
      font-family: sans-serif;
    }
  `]
})
export class PurchaseBillComponent {}
