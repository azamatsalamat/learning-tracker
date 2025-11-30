'use client';

import { useEffect, useState, FormEvent } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '../context/AuthContext';
import { getProfile, updateProfile, getUserIdFromToken, Profile, Experience, Education, PersonalProject, Certification, Publication, Award } from '@/lib/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

export default function ProfilePage() {
  const { isAuthenticated, token, logout } = useAuth();
  const router = useRouter();

  const [profile, setProfile] = useState<Profile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [city, setCity] = useState('');
  const [country, setCountry] = useState('');
  const [summary, setSummary] = useState('');
  const [skills, setSkills] = useState('');
  const [languages, setLanguages] = useState('');
  const [experiences, setExperiences] = useState<Experience[]>([]);
  const [educations, setEducations] = useState<Education[]>([]);
  const [personalProjects, setPersonalProjects] = useState<PersonalProject[]>([]);
  const [certifications, setCertifications] = useState<Certification[]>([]);
  const [publications, setPublications] = useState<Publication[]>([]);
  const [awards, setAwards] = useState<Award[]>([]);

  useEffect(() => {
    if (!isAuthenticated || !token) {
      router.push('/login');
      return;
    }

    const loadProfile = async () => {
      try {
        const userId = getUserIdFromToken(token);
        const data = await getProfile(userId, token);
        
        if (!data) {
          router.push('/profile/create');
          return;
        }

        setProfile(data);
        setFirstName(data.name?.firstName || '');
        setLastName(data.name?.lastName || '');
        setEmail(data.email || '');
        setPhone(data.phone || '');
        setCity(data.address?.city || '');
        setCountry(data.address?.country || '');
        setSummary(data.summary || '');
        setSkills(data.skills?.join(', ') || '');
        setLanguages(data.languages?.join(', ') || '');
        setExperiences(data.experiences || []);
        setEducations(data.educations || []);
        setPersonalProjects(data.personalProjects || []);
        setCertifications(data.certifications || []);
        setPublications(data.publications || []);
        setAwards(data.awards || []);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load profile');
      } finally {
        setIsLoading(false);
      }
    };

    loadProfile();
  }, [isAuthenticated, token, router]);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setIsSaving(true);

    try {
      if (!profile || !token) return;

      const updateData = {
        id: profile.id,
        name: firstName || lastName ? { firstName, lastName } : undefined,
        email: email || undefined,
        phone: phone || undefined,
        address: city || country ? { city, country } : undefined,
        summary: summary || undefined,
        skills: skills ? skills.split(',').map(s => s.trim()).filter(s => s) : [],
        languages: languages ? languages.split(',').map(l => l.trim()).filter(l => l) : [],
        experiences: experiences.length > 0 ? experiences : undefined,
        educations: educations.length > 0 ? educations : undefined,
        personalProjects: personalProjects.length > 0 ? personalProjects : undefined,
        certifications: certifications.length > 0 ? certifications : undefined,
        publications: publications.length > 0 ? publications : undefined,
        awards: awards.length > 0 ? awards : undefined,
      };

      await updateProfile(updateData, token);
      setSuccess('Profile updated successfully!');
      
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  const addExperience = () => {
    setExperiences([...experiences, {
      company: '',
      position: '',
      description: '',
      startDate: '',
      endDate: undefined,
      technologies: [],
      responsibilities: [],
      achievements: []
    }]);
  };

  const updateExperience = (index: number, field: keyof Experience, value: string | string[]) => {
    const updated = [...experiences];
    updated[index] = { ...updated[index], [field]: value };
    setExperiences(updated);
  };

  const removeExperience = (index: number) => {
    setExperiences(experiences.filter((_, i) => i !== index));
  };

  const addEducation = () => {
    setEducations([...educations, {
      school: '',
      degree: '',
      major: '',
      startDate: '',
      endDate: undefined,
      courses: [],
      achievements: []
    }]);
  };

  const updateEducation = (index: number, field: keyof Education, value: string | string[]) => {
    const updated = [...educations];
    updated[index] = { ...updated[index], [field]: value };
    setEducations(updated);
  };

  const removeEducation = (index: number) => {
    setEducations(educations.filter((_, i) => i !== index));
  };

  const addPersonalProject = () => {
    setPersonalProjects([...personalProjects, {
      name: '',
      description: '',
      technologies: []
    }]);
  };

  const updatePersonalProject = (index: number, field: keyof PersonalProject, value: string | string[]) => {
    const updated = [...personalProjects];
    updated[index] = { ...updated[index], [field]: value };
    setPersonalProjects(updated);
  };

  const removePersonalProject = (index: number) => {
    setPersonalProjects(personalProjects.filter((_, i) => i !== index));
  };

  const addCertification = () => {
    setCertifications([...certifications, {
      name: '',
      issuer: '',
      issueDate: '',
      expirationDate: undefined,
      credentialId: undefined,
      credentialUrl: undefined
    }]);
  };

  const updateCertification = (index: number, field: keyof Certification, value: string | undefined) => {
    const updated = [...certifications];
    updated[index] = { ...updated[index], [field]: value };
    setCertifications(updated);
  };

  const removeCertification = (index: number) => {
    setCertifications(certifications.filter((_, i) => i !== index));
  };

  const addPublication = () => {
    setPublications([...publications, {
      title: '',
      description: '',
      authors: [],
      link: undefined
    }]);
  };

  const updatePublication = (index: number, field: keyof Publication, value: string | string[] | undefined) => {
    const updated = [...publications];
    updated[index] = { ...updated[index], [field]: value };
    setPublications(updated);
  };

  const removePublication = (index: number) => {
    setPublications(publications.filter((_, i) => i !== index));
  };

  const addAward = () => {
    setAwards([...awards, {
      name: '',
      issuer: '',
      date: '',
      description: undefined
    }]);
  };

  const updateAward = (index: number, field: keyof Award, value: string | undefined) => {
    const updated = [...awards];
    updated[index] = { ...updated[index], [field]: value };
    setAwards(updated);
  };

  const removeAward = (index: number) => {
    setAwards(awards.filter((_, i) => i !== index));
  };

  if (!isAuthenticated) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-muted/50">
        <div className="text-muted-foreground">Loading...</div>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-muted/50">
        <div className="text-muted-foreground">Loading profile...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-muted/50">
      <nav className="bg-background border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <h1 className="text-xl font-semibold">Profile</h1>
            <Button variant="ghost" onClick={logout}>
              Logout
            </Button>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Card>
          <CardHeader>
            <CardTitle>Edit Your Profile</CardTitle>
            <CardDescription>
              Update your personal information and skills
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-4">
                <h3 className="text-lg font-semibold">Personal Information</h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="firstName">First Name</Label>
                    <Input
                      id="firstName"
                      type="text"
                      value={firstName}
                      onChange={(e) => setFirstName(e.target.value)}
                      placeholder="John"
                    />
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="lastName">Last Name</Label>
                    <Input
                      id="lastName"
                      type="text"
                      value={lastName}
                      onChange={(e) => setLastName(e.target.value)}
                      placeholder="Doe"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="email">Email</Label>
                  <Input
                    id="email"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="john.doe@example.com"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="phone">Phone</Label>
                  <Input
                    id="phone"
                    type="tel"
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="+1 234 567 8900"
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="city">City</Label>
                    <Input
                      id="city"
                      type="text"
                      value={city}
                      onChange={(e) => setCity(e.target.value)}
                      placeholder="New York"
                    />
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="country">Country</Label>
                    <Input
                      id="country"
                      type="text"
                      value={country}
                      onChange={(e) => setCountry(e.target.value)}
                      placeholder="USA"
                    />
                  </div>
                </div>
              </div>

              <div className="space-y-4">
                <h3 className="text-lg font-semibold">Professional Information</h3>
                
                <div className="space-y-2">
                  <Label htmlFor="summary">Summary</Label>
                  <Textarea
                    id="summary"
                    value={summary}
                    onChange={(e) => setSummary(e.target.value)}
                    placeholder="Tell us about yourself..."
                    rows={4}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="skills">Skills</Label>
                  <Input
                    id="skills"
                    type="text"
                    value={skills}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setSkills(e.target.value)}
                    placeholder="JavaScript, React, Node.js, Python"
                  />
                  <p className="text-xs text-muted-foreground">
                    Separate skills with commas
                  </p>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="languages">Languages</Label>
                  <Input
                    id="languages"
                    type="text"
                    value={languages}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setLanguages(e.target.value)}
                    placeholder="English, Spanish, French"
                  />
                  <p className="text-xs text-muted-foreground">
                    Separate languages with commas
                  </p>
                </div>
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Experience</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addExperience}>
                    Add Experience
                  </Button>
                </div>
                
                {experiences.map((exp, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Experience {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removeExperience(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Company</Label>
                          <Input
                            value={exp.company}
                            onChange={(e) => updateExperience(index, 'company', e.target.value)}
                            placeholder="Company name"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Position</Label>
                          <Input
                            value={exp.position}
                            onChange={(e) => updateExperience(index, 'position', e.target.value)}
                            placeholder="Job title"
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label>Description</Label>
                        <Textarea
                          value={exp.description}
                          onChange={(e) => updateExperience(index, 'description', e.target.value)}
                          placeholder="Describe your role..."
                          rows={3}
                        />
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Start Date</Label>
                          <Input
                            type="date"
                            value={exp.startDate}
                            onChange={(e) => updateExperience(index, 'startDate', e.target.value)}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>End Date (Optional)</Label>
                          <Input
                            type="date"
                            value={exp.endDate || ''}
                            onChange={(e) => updateExperience(index, 'endDate', e.target.value || undefined)}
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label>Technologies</Label>
                        <Input
                          value={exp.technologies.join(', ')}
                          onChange={(e) => updateExperience(index, 'technologies', e.target.value.split(',').map(t => t.trim()).filter(t => t))}
                          placeholder="React, Node.js, PostgreSQL"
                        />
                        <p className="text-xs text-muted-foreground">Separate with commas</p>
                      </div>

                      <div className="space-y-2">
                        <Label>Responsibilities</Label>
                        <Textarea
                          value={exp.responsibilities.join('\n')}
                          onChange={(e) => updateExperience(index, 'responsibilities', e.target.value.split('\n').filter(r => r.trim()))}
                          placeholder="One responsibility per line"
                          rows={3}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Achievements</Label>
                        <Textarea
                          value={exp.achievements.join('\n')}
                          onChange={(e) => updateExperience(index, 'achievements', e.target.value.split('\n').filter(a => a.trim()))}
                          placeholder="One achievement per line"
                          rows={3}
                        />
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Education</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addEducation}>
                    Add Education
                  </Button>
                </div>
                
                {educations.map((edu, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Education {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removeEducation(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>School</Label>
                          <Input
                            value={edu.school}
                            onChange={(e) => updateEducation(index, 'school', e.target.value)}
                            placeholder="University name"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Degree</Label>
                          <Input
                            value={edu.degree}
                            onChange={(e) => updateEducation(index, 'degree', e.target.value)}
                            placeholder="Bachelor's, Master's, etc."
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label>Major</Label>
                        <Input
                          value={edu.major}
                          onChange={(e) => updateEducation(index, 'major', e.target.value)}
                          placeholder="Computer Science, etc."
                        />
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Start Date</Label>
                          <Input
                            type="date"
                            value={edu.startDate}
                            onChange={(e) => updateEducation(index, 'startDate', e.target.value)}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>End Date (Optional)</Label>
                          <Input
                            type="date"
                            value={edu.endDate || ''}
                            onChange={(e) => updateEducation(index, 'endDate', e.target.value || undefined)}
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label>Courses</Label>
                        <Textarea
                          value={edu.courses.join('\n')}
                          onChange={(e) => updateEducation(index, 'courses', e.target.value.split('\n').filter(c => c.trim()))}
                          placeholder="One course per line"
                          rows={3}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Achievements</Label>
                        <Textarea
                          value={edu.achievements.join('\n')}
                          onChange={(e) => updateEducation(index, 'achievements', e.target.value.split('\n').filter(a => a.trim()))}
                          placeholder="One achievement per line"
                          rows={3}
                        />
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Personal Projects</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addPersonalProject}>
                    Add Project
                  </Button>
                </div>
                
                {personalProjects.map((project, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Project {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removePersonalProject(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="space-y-2">
                        <Label>Project Name</Label>
                        <Input
                          value={project.name}
                          onChange={(e) => updatePersonalProject(index, 'name', e.target.value)}
                          placeholder="Project name"
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Description</Label>
                        <Textarea
                          value={project.description}
                          onChange={(e) => updatePersonalProject(index, 'description', e.target.value)}
                          placeholder="Describe your project..."
                          rows={3}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Technologies</Label>
                        <Input
                          value={project.technologies.join(', ')}
                          onChange={(e) => updatePersonalProject(index, 'technologies', e.target.value.split(',').map(t => t.trim()).filter(t => t))}
                          placeholder="React, Python, etc."
                        />
                        <p className="text-xs text-muted-foreground">Separate with commas</p>
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Certifications</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addCertification}>
                    Add Certification
                  </Button>
                </div>
                
                {certifications.map((cert, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Certification {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removeCertification(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Certification Name</Label>
                          <Input
                            value={cert.name}
                            onChange={(e) => updateCertification(index, 'name', e.target.value)}
                            placeholder="AWS Certified Developer"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Issuer</Label>
                          <Input
                            value={cert.issuer}
                            onChange={(e) => updateCertification(index, 'issuer', e.target.value)}
                            placeholder="Amazon Web Services"
                          />
                        </div>
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Issue Date</Label>
                          <Input
                            type="date"
                            value={cert.issueDate}
                            onChange={(e) => updateCertification(index, 'issueDate', e.target.value)}
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Expiration Date (Optional)</Label>
                          <Input
                            type="date"
                            value={cert.expirationDate || ''}
                            onChange={(e) => updateCertification(index, 'expirationDate', e.target.value || undefined)}
                          />
                        </div>
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Credential ID (Optional)</Label>
                          <Input
                            value={cert.credentialId || ''}
                            onChange={(e) => updateCertification(index, 'credentialId', e.target.value || undefined)}
                            placeholder="ABC123XYZ"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Credential URL (Optional)</Label>
                          <Input
                            value={cert.credentialUrl || ''}
                            onChange={(e) => updateCertification(index, 'credentialUrl', e.target.value || undefined)}
                            placeholder="https://..."
                          />
                        </div>
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Publications</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addPublication}>
                    Add Publication
                  </Button>
                </div>
                
                {publications.map((pub, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Publication {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removePublication(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="space-y-2">
                        <Label>Title</Label>
                        <Input
                          value={pub.title}
                          onChange={(e) => updatePublication(index, 'title', e.target.value)}
                          placeholder="Publication title"
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Description</Label>
                        <Textarea
                          value={pub.description}
                          onChange={(e) => updatePublication(index, 'description', e.target.value)}
                          placeholder="Describe the publication..."
                          rows={3}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Authors</Label>
                        <Input
                          value={pub.authors.join(', ')}
                          onChange={(e) => updatePublication(index, 'authors', e.target.value.split(',').map(a => a.trim()).filter(a => a))}
                          placeholder="John Doe, Jane Smith"
                        />
                        <p className="text-xs text-muted-foreground">Separate with commas</p>
                      </div>

                      <div className="space-y-2">
                        <Label>Link (Optional)</Label>
                        <Input
                          value={pub.link || ''}
                          onChange={(e) => updatePublication(index, 'link', e.target.value || undefined)}
                          placeholder="https://..."
                        />
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-lg font-semibold">Awards</h3>
                  <Button type="button" variant="outline" size="sm" onClick={addAward}>
                    Add Award
                  </Button>
                </div>
                
                {awards.map((award, index) => (
                  <Card key={index} className="p-4">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <h4 className="font-medium text-sm text-muted-foreground">Award {index + 1}</h4>
                        <Button type="button" variant="ghost" size="sm" onClick={() => removeAward(index)}>
                          Remove
                        </Button>
                      </div>
                      
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div className="space-y-2">
                          <Label>Award Name</Label>
                          <Input
                            value={award.name}
                            onChange={(e) => updateAward(index, 'name', e.target.value)}
                            placeholder="Employee of the Year"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label>Issuer</Label>
                          <Input
                            value={award.issuer}
                            onChange={(e) => updateAward(index, 'issuer', e.target.value)}
                            placeholder="Company name"
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label>Date</Label>
                        <Input
                          type="date"
                          value={award.date}
                          onChange={(e) => updateAward(index, 'date', e.target.value)}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Description (Optional)</Label>
                        <Textarea
                          value={award.description || ''}
                          onChange={(e) => updateAward(index, 'description', e.target.value || undefined)}
                          placeholder="Describe the award..."
                          rows={2}
                        />
                      </div>
                    </div>
                  </Card>
                ))}
              </div>

              {error && (
                <div className="text-sm text-destructive bg-destructive/10 p-3 rounded-md">
                  {error}
                </div>
              )}

              {success && (
                <div className="text-sm text-green-600 bg-green-50 dark:bg-green-950/30 p-3 rounded-md">
                  {success}
                </div>
              )}

              <Button type="submit" disabled={isSaving} className="w-full">
                {isSaving ? 'Saving...' : 'Save Changes'}
              </Button>
            </form>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
