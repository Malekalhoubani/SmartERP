import {
  Directive,
  effect,
  inject,
  Input,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';

import { AuthStateService } from '../../core/services/auth-state.service';

@Directive({
  selector: '[appHasPermission]',
  standalone: true,
})
export class HasPermissionDirective {
  private readonly template = inject(TemplateRef<unknown>);
  private readonly viewContainer = inject(ViewContainerRef);
  private readonly authState = inject(AuthStateService);
  private permission = '';
  private hasView = false;

  constructor() {
    effect(() => {
      this.authState.permissions();
      this.updateView();
    });
  }

  @Input()
  set appHasPermission(permission: string) {
    this.permission = permission;
    this.updateView();
  }

  private updateView(): void {
    const canView =
      this.permission.length > 0 &&
      this.authState.hasPermission(this.permission);

    if (canView && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.template);
      this.hasView = true;
    } else if (!canView && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
