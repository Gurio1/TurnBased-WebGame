import { AfterViewInit, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-defeat-modal',
  standalone: true,
  imports: [],
  templateUrl: './defeat-modal.component.html',
  styleUrl: './defeat-modal.component.scss',
})
export class DefeatModalComponent implements AfterViewInit {
  constructor(private router: Router) {}
  ngAfterViewInit(): void {
    const audio = new Audio('/sounds/creepy_skull_laugh.mp3');
    audio.play();
  }
  goHome() {
    this.router.navigate(['/home']);
  }
  closeModal(): void {}
}
