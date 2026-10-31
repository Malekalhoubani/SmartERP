import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { TranslationService } from '../../../core/i18n/translation.service';
import { CompanyContextService } from '../../../core/services/company-context.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet,RouterLink,RouterLinkActive],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
})
export class AppShellComponent {
  readonly translation = inject(TranslationService);
  readonly companyContext = inject(CompanyContextService);
  readonly menuOpen = signal(false);
}
