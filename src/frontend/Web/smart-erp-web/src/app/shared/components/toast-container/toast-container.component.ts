import { Component, inject } from '@angular/core';
import { TranslationService } from '../../../core/i18n/translation.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  templateUrl: './toast-container.component.html',
  styleUrl: './toast-container.component.scss',
})
export class ToastContainerComponent {
  readonly toastService = inject(ToastService);
  readonly translation = inject(TranslationService);
}
