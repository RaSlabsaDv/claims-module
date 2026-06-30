import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="status-badge" [attr.data-status]="status()">{{ status() }}</span>`,
  styleUrl: './status-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StatusBadgeComponent {
  readonly status = input.required<string>();
}
